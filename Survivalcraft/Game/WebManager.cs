using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine;
using SimpleJson;

namespace Game
{
	// Token: 0x02000356 RID: 854
	public static class WebManager
	{
		// Token: 0x06001808 RID: 6152
		[DllImport("wininet.dll")]
		public static extern bool InternetGetConnectedState(out int Description, int ReservedValue);

		// Token: 0x06001809 RID: 6153 RVA: 0x000BE18C File Offset: 0x000BC38C
		public static bool IsInternetConnectionAvailable()
		{
			try
			{
				int num;
				return WebManager.InternetGetConnectedState(out num, 0);
			}
			catch (Exception e)
			{
				Log.Warning(ExceptionManager.MakeFullErrorMessage("Could not check internet connection availability.", e));
			}
			return true;
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x000BE1CC File Offset: 0x000BC3CC
		public static void Get(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
            Task.Run((Func<Task>)(async () =>
            {
                try
                {
                    progress = progress ?? new CancellableProgress();
                    if (!WebManager.IsInternetConnectionAvailable())
                        throw new InvalidOperationException("Internet connection is unavailable.");
                    using (HttpClient client = new HttpClient())
                    {
                        Uri uri;
                        if (parameters == null || parameters.Count <= 0)
                            uri = new Uri(address);
                        else
                            uri = new Uri(string.Format("{0}?{1}", new object[2]
                            {
                (object) address,
                (object) WebManager.UrlParametersToString(parameters)
                            }));
                        Uri requestUri = uri;
                        client.DefaultRequestHeaders.Referrer = new Uri(address);
                        if (headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in headers)
                                client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                        HttpResponseMessage responseMessage = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, progress.CancellationToken);
                        await WebManager.VerifyResponse(responseMessage);
                        long? contentLength = responseMessage.Content.Headers.ContentLength;
                        progress.Total = (float)contentLength.GetValueOrDefault();
                        using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                        {
                            using (MemoryStream targetStream = new MemoryStream())
                            {
                                long written = 0;
                                byte[] buffer = new byte[1024];
                                int count;
                                do
                                {
                                    count = await responseStream.ReadAsync(buffer, 0, buffer.Length, progress.CancellationToken);
                                    if (count > 0)
                                    {
                                        targetStream.Write(buffer, 0, count);
                                        written += (long)count;
                                        progress.Completed = (float)written;
                                    }
                                }
                                while (count > 0);
                                Dispatcher.Dispatch((Action)(() => success(targetStream.ToArray())));
                                buffer = (byte[])null;
                            }
                        }
                        responseMessage = (HttpResponseMessage)null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ExceptionManager.MakeFullErrorMessage(ex));
                    Dispatcher.Dispatch((Action)(() => failure(ex)));
                }
            }));
        }

		// Token: 0x0600180B RID: 6155 RVA: 0x000BE229 File Offset: 0x000BC429
		public static void Put(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			WebManager.PutOrPost(false, address, parameters, headers, data, progress, success, failure);
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x000BE23B File Offset: 0x000BC43B
		public static void Post(string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
			WebManager.PutOrPost(true, address, parameters, headers, data, progress, success, failure);
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x000BE250 File Offset: 0x000BC450
		public static string UrlParametersToString(Dictionary<string, string> values)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = string.Empty;
			foreach (KeyValuePair<string, string> keyValuePair in values)
			{
				stringBuilder.Append(value);
				value = "&";
				stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Key));
				stringBuilder.Append('=');
				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					stringBuilder.Append(Uri.EscapeDataString(keyValuePair.Value));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x000BE2F4 File Offset: 0x000BC4F4
		public static byte[] UrlParametersToBytes(Dictionary<string, string> values)
		{
			return Encoding.UTF8.GetBytes(WebManager.UrlParametersToString(values));
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x000BE306 File Offset: 0x000BC506
		public static MemoryStream UrlParametersToStream(Dictionary<string, string> values)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(WebManager.UrlParametersToString(values)));
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x000BE320 File Offset: 0x000BC520
		public static Dictionary<string, string> UrlParametersFromString(string s)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = s.Split(new char[]
			{
				'&'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = Uri.UnescapeDataString(array[i]).Split(new char[]
				{
					'='
				});
				if (array2.Length == 2)
				{
					dictionary[array2[0]] = array2[1];
				}
			}
			return dictionary;
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x000BE380 File Offset: 0x000BC580
		public static Dictionary<string, string> UrlParametersFromBytes(byte[] bytes)
		{
			return WebManager.UrlParametersFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x000BE396 File Offset: 0x000BC596
		public static object JsonFromString(string s)
		{
			return SimpleJson.SimpleJson.DeserializeObject(s);
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x000BE39E File Offset: 0x000BC59E
		public static object JsonFromBytes(byte[] bytes)
		{
			return WebManager.JsonFromString(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x000BE3B4 File Offset: 0x000BC5B4
		public static void PutOrPost(bool isPost, string address, Dictionary<string, string> parameters, Dictionary<string, string> headers, Stream data, CancellableProgress progress, Action<byte[]> success, Action<Exception> failure)
		{
            Task.Run((Func<Task>)(async () =>
            {
                try
                {
                    if (!WebManager.IsInternetConnectionAvailable())
                        throw new InvalidOperationException("Internet connection is unavailable.");
                    using (HttpClient client = new HttpClient())
                    {
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        if (headers != null)
                        {
                            foreach (KeyValuePair<string, string> header in headers)
                            {
                                if (!client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value))
                                    dictionary.Add(header.Key, header.Value);
                            }
                        }
                        Uri uri;
                        if (parameters == null || parameters.Count <= 0)
                            uri = new Uri(address);
                        else
                            uri = new Uri(string.Format("{0}?{1}", new object[2]
                            {
                (object) address,
                (object) WebManager.UrlParametersToString(parameters)
                            }));
                        Uri requestUri = uri;
                        WebManager.ProgressHttpContent content = new WebManager.ProgressHttpContent(data, progress);
                        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                            content.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                        HttpResponseMessage httpResponseMessage;
                        if (!isPost)
                            httpResponseMessage = await client.PutAsync(requestUri, (HttpContent)content, progress.CancellationToken);
                        else
                            httpResponseMessage = await client.PostAsync(requestUri, (HttpContent)content, progress.CancellationToken);
                        HttpResponseMessage responseMessage = httpResponseMessage;
                        await WebManager.VerifyResponse(responseMessage);
                        byte[] responseData = await responseMessage.Content.ReadAsByteArrayAsync();
                        Dispatcher.Dispatch((Action)(() => success(responseData)));
                        responseMessage = (HttpResponseMessage)null;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ExceptionManager.MakeFullErrorMessage(ex));
                    Dispatcher.Dispatch((Action)(() => failure(ex)));
                }
            }));
        }

		// Token: 0x06001815 RID: 6165 RVA: 0x000BE424 File Offset: 0x000BC624
		public static async Task VerifyResponse(HttpResponseMessage message)
		{
            if (!message.IsSuccessStatusCode)
            {
                string responseText = string.Empty;
                try
                {
                    responseText = await message.Content.ReadAsStringAsync();
                }
                catch
                {
                }
                throw new InvalidOperationException(string.Format("{0} ({1})\n{2}", new object[3]
                {
          (object) message.StatusCode.ToString(),
          (object) (int) message.StatusCode,
          (object) responseText
                }));
            }
        }

		// Token: 0x02000506 RID: 1286
		public class ProgressHttpContent : HttpContent
		{
			// Token: 0x060020F6 RID: 8438 RVA: 0x000E5EED File Offset: 0x000E40ED
			public ProgressHttpContent(Stream sourceStream, CancellableProgress progress)
			{
				this.m_sourceStream = sourceStream;
				this.m_progress = (progress ?? new CancellableProgress());
			}

			// Token: 0x060020F7 RID: 8439 RVA: 0x000E5F0C File Offset: 0x000E410C
			protected override bool TryComputeLength(out long length)
			{
				length = this.m_sourceStream.Length;
				return true;
			}

			// Token: 0x060020F8 RID: 8440 RVA: 0x000E5F1C File Offset: 0x000E411C
			protected override async Task SerializeToStreamAsync(Stream targetStream, TransportContext context)
			{
                byte[] buffer = new byte[1024];
                long written = 0;
                int read;
                do
                {
                    this.m_progress.Total = (float)this.m_sourceStream.Length;
                    this.m_progress.Completed = (float)written;
                    if (!this.m_progress.CancellationToken.IsCancellationRequested)
                    {
                        read = this.m_sourceStream.Read(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            await targetStream.WriteAsync(buffer, 0, read, this.m_progress.CancellationToken);
                            written += (long)read;
                        }
                    }
                    else
                        goto label_5;
                }
                while (read > 0);
                goto label_6;
            label_5:
                throw new OperationCanceledException("Operation cancelled.");
            label_6:
                buffer = (byte[])null;
            }

			// Token: 0x04001893 RID: 6291
			public Stream m_sourceStream;

			// Token: 0x04001894 RID: 6292
			public CancellableProgress m_progress;
		}
	}
}
