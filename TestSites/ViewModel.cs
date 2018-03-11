using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TestSites;

public class ViewModel : INotifyPropertyChanged
{
    private string name;
    private string name2;
    private const string test_str = @"addnew?display=0&referer=https%3A%2F%2Fsite1.tomnolane.ru%2F&step=3&ad_id=4&amount=20000&period=21&rangeSlider=20000&f=%D0%A2%D0%B5%D1%81%D1%82&i=%D0%A2%D0%B5%D1%81%D1%82&o=%D0%A2%D0%B5%D1%81%D1%82&gender=0&birth_dd=0&birth_mm=0&birth_yyyy=0&birthdate=27%2F01%2F1999&phone=8+(912)+345+6789&email=test%40test.tu&delays_type=never&passport=1234+567890&passport_s=1234&passport_n=567890&passportdate=05%2F01%2F2018&passport_code=770-095&passport_who=%D0%9E%D0%A2%D0%94%D0%95%D0%9B+%D0%A3%D0%A4%D0%9C%D0%A1+%D0%A0%D0%9E%D0%A1%D0%A1%D0%98%D0%98+%D0%9F%D0%9E+%D0%93%D0%9E%D0%A0.+%D0%9C%D0%9E%D0%A1%D0%9A%D0%92%D0%95+%D0%9F%D0%9E+%D0%A0%D0%90%D0%99%D0%9E%D0%9D%D0%A3+%D0%A1%D0%95%D0%92%D0%95%D0%A0%D0%9D%D0%9E%D0%95+%D0%A2%D0%A3%D0%A8%D0%98%D0%9D%D0%9E&birthplace=%D0%A2%D0%B5%D1%81%D1%82&region=%D0%A0%D0%B5%D1%81%D0%BF%D1%83%D0%B1%D0%BB%D0%B8%D0%BA%D0%B0+%D0%9A%D0%B0%D1%80%D0%B5%D0%BB%D0%B8%D1%8F&city=%D0%9F%D0%B5%D1%82%D1%80%D0%BE%D0%B7%D0%B0%D0%B2%D0%BE%D0%B4%D1%81%D0%BA&street=%D0%A2%D0%B5%D1%81%D1%82&building=1&housing=2&flat=3&reg_type=1&reg_same=1&work=%D0%9F%D0%95%D0%9D%D0%A1%D0%98%D0%9E%D0%9D%D0%95%D0%A0&work_name=%D0%BF%D0%B5%D0%BD%D1%81%D0%B8%D0%BE%D0%BD%D0%B5%D1%80&work_occupation=%D0%BF%D0%B5%D0%BD%D1%81%D0%B8%D0%BE%D0%BD%D0%B5%D1%80&work_phone=8+(912)+345+6789&work_experience=100&work_salary=123456&work_region=%D0%A0%D0%B5%D1%81%D0%BF%D1%83%D0%B1%D0%BB%D0%B8%D0%BA%D0%B0+%D0%9A%D0%B0%D1%80%D0%B5%D0%BB%D0%B8%D1%8F&work_city=%D0%9F%D0%B5%D1%82%D1%80%D0%BE%D0%B7%D0%B0%D0%B2%D0%BE%D0%B4%D1%81%D0%BA&work_street=%D0%A2%D0%B5%D1%81%D1%82&work_house=1&work_office=3";
    public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении

    public void RaisePropertyChanged(string propertyName)
    {
        // Если кто-то на него подписан, то вызывем его
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // А тут будут свойства, в которые мы обернем поля
    public string Name
    {
        get { return name; }
        set
        {
            // Устанавливаем новое значение
            name = value;
            // Сообщаем всем, кто подписан на событие PropertyChanged, что поле изменилось Name
            RaisePropertyChanged("Name");
        }
    }

    public string Name2
    {
        get { return name2; }
        set
        {
            // Устанавливаем новое значение
            name2 = value;
            // Сообщаем всем, кто подписан на событие PropertyChanged, что поле изменилось Name
            RaisePropertyChanged("Name2");
        }
    }

    public async Task<Dictionary<string, string>> LoadSites()
    {
        var temp = await GET("https://tomnolane.ru/out/sites.txt", false);

        Dictionary<string, string> dict = new Dictionary<string, string>();
        foreach (var t in temp.Split('\n'))
        {
            if(!string.IsNullOrWhiteSpace(t.Split(' ')[0]) && !string.IsNullOrWhiteSpace(t.Split(' ')[1]))
                dict.Add(t.Split(' ')[0], t.Split(' ')[1]);
        }
        return dict;
    } 


    public async void ButtonClicked()
    {
        var temp = await LoadSites();
        Errors er = new Errors();
        int page_count = 0; 
        foreach (var t in temp)
        { 
            try
            {
                XmlDocument doc = new XmlDocument();
                Name += "Идет проверка " + t.Key + "\n";
                var r = await GET(t.Value);
                doc.LoadXml(r);
                XmlNodeList urlNodes = doc.GetElementsByTagName("loc");
                Name2 = "Проверено 0 из " + urlNodes.Count + " страниц";
                int count_page_site = 0;
                foreach (XmlNode url in urlNodes)
                {
                    int count = 0;
                    K:
                    Thread.Sleep(200);

                    var r2 = await GET(url.InnerText);

                    if (string.IsNullOrEmpty(r2))
                        if (count < 3)
                        {
                            count++;
                            goto K;
                        }
                        else
                        {
                            if (r2.ToLower().Contains("произошла ошибка безопасности"))
                            {
                                er.AddList(new Error { SiteName = "Сайт " + t.Key, ErrorName = "Отсутствует SSL", UrlName = url.InnerText });
                            }
                            else
                            {
                                er.AddList(new Error { SiteName = "Сайт " + t.Key, ErrorName = "Страница отсутствует", UrlName = url.InnerText });
                            }
                        }
                    else if (r2.ToLower().Contains("php"))
                    {
                        er.AddList(new Error { SiteName = "Сайт " + t.Key, ErrorName = "Ошибка backend PHP", UrlName = url.InnerText });
                    }
                    page_count++;
                    count_page_site++;
                    Name2 = "Проверено " + count_page_site + " из " + urlNodes.Count + " страниц";


                    if (count_page_site == urlNodes.Count)
                    {
                        r = await GET("https://" + t.Key + "/" + test_str);
                        if (!r.Contains("\"redirect\":"))
                        {
                            er.AddList(new Error { SiteName = "Сайт " + t.Key, ErrorName = "Анкета не работает", UrlName = "https://" + t.Key + "/form" });
                        }
                    }
                }
            }
            catch
            {
                er.AddList(new Error { SiteName = "Сайт " + t.Key, ErrorName = "Ошибка: проблема с сайтов. Срочно проверить.", UrlName = t.Key });
            }
        }

        Name += "Проверка завершена! Проверено страниц: " + page_count + "\n";
        if (er.GetList().Count > 0)
        {
            Name += "Обнаружено ошибок: " + er.GetList().Count+ "\n";
            foreach(var t in er.GetList())
            {
                Name += t.SiteName + " Ошибка: " + t.ErrorName + " Страница: " + t.UrlName + "\n";
            }
        }
        else
        {
            Name += "Ошибок не обнаружено!";
        }
    }

    private async Task<string> GET(string url, bool t = true)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0";
            request.AllowAutoRedirect = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            if(t)
                request.Accept = "text/html,application/json,application/xml;q=0.9,*/*;q=0.8";
            else request.Accept = "text/plain";
            return await Task.Run(() =>
            {
                string resp = RESPONSE(request);
                return resp;
            });
        }
        catch (Exception ex)
        {
            return ex.Message + "\n" + ex.StackTrace;
        }
    }

    private string RESPONSE(HttpWebRequest request)
    {
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string answer = "";
            var headers = response.Headers.ToString();

            if (Convert.ToInt32(response.StatusCode) == 302 || Convert.ToInt32(response.StatusCode) == 200)
            {
                using (Stream rspStm = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(rspStm, Encoding.UTF8, true))
                    {
                        answer = string.Empty; answer = reader.ReadToEnd();
                    }
                }
                return answer;
            }
            else
            {
                response.Close(); return WebUtility.HtmlDecode(response.StatusDescription);
            }
        }
        catch (Exception ex)
        {
            return WebUtility.HtmlDecode(ex.Message) + "\n" + ex.StackTrace;
        }
    }

}