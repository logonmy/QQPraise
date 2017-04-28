using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace QQVeryCool.Util
{
    public static class ScrapySharpExtession
    {
        public static string NavigateTo(this ScrapingBrowser scrapingBrowser, string url, NameValueCollection data,
                                        Action<ScrapingBrowser, string> action, HttpVerb httpVerb = HttpVerb.Get)
        {
            var html = scrapingBrowser.NavigateTo(new Uri(url), httpVerb, data);
            action.Invoke(scrapingBrowser, html);
            return html;
        }

        public static string DownloadString(this ScrapingBrowser scrapingBrowser, string url, Action<ScrapingBrowser, string> action)
        {
            var html = scrapingBrowser.DownloadString(new Uri(url));
            action.Invoke(scrapingBrowser, html);
            return html;
        }

        public static IEnumerable<string> ExactHref(this string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var htmlNode = htmlDocument.DocumentNode;

            var nodes = htmlNode.SelectNodes("//a/@href");
            return nodes.Select(node => HtmlParsingHelper.GetAttributeValue(node, "href"));
        }

        public static IEnumerable<string> ExactOnclick(this string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var htmlNode = htmlDocument.DocumentNode;

            var nodes = htmlNode.SelectNodes("//a/@onclick");
            return nodes.Select(node => node.GetAttributeValue("onclick"));
        }
    }

    public static class CharAtExtention
    {
        public static string CharAt(this string s, int index)
        {
            if ((index >= s.Length) || (index < 0))
                return "";
            return s.Substring(index, 1);
        }
    }
}