﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsQuery;
using Jackett.Common.Models;
using Jackett.Common.Models.IndexerConfig.Bespoke;
using Jackett.Common.Services.Interfaces;
using Jackett.Common.Utils;
using Jackett.Common.Utils.Clients;
using Newtonsoft.Json.Linq;
using NLog;

namespace Jackett.Common.Indexers
{
    public class TVstore : BaseWebIndexer
    {

        private string LoginUrl { get { return SiteLink + "takelogin.php"; } }
        private string loginPageUrl { get { return SiteLink + "login.php?returnto=%2F"; } }
        private string SearchUrl { get { return SiteLink + "torrent/br_process.php"; } }
<<<<<<< HEAD
        private List<SeriesDetail> series = new List<SeriesDetail>();
=======
        private List<SeriesDetail> series = new List<SeriesDetail>(); 
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11

        private new ConfigurationDataTVstore configData
        {
            get { return (ConfigurationDataTVstore)base.configData; }
            set { base.configData = value; }
        }

        public TVstore(IIndexerConfigurationService configService, WebClient wc, Logger l, IProtectionService ps)
            : base(name: "TVstore",
                description: "A Hungarian private torrent site.",
                link: "https://tvstore.me/",
                caps: new TorznabCapabilities(),
                configService: configService,
                client: wc,
                logger: l,
                p: ps,
                configData: new ConfigurationDataTVstore())
        {
            Encoding = Encoding.UTF8;
            Language = "hu-hu";
            Type = "private";

            AddCategoryMapping(1, TorznabCatType.TV);

        }

        public override async Task<IndexerConfigurationStatus> ApplyConfiguration(JToken configJson)
        {
            LoadValuesFromJson(configJson);

            var loginPage = await RequestStringWithCookies(loginPageUrl, string.Empty);
            var pairs = new Dictionary<string, string> {
                { "username", configData.Username.Value },
                { "password", configData.Password.Value },
                { "back", "%2F" },
                { "logout", "1"}
            };

<<<<<<< HEAD
            var result = await RequestLoginAndFollowRedirect(LoginUrl, pairs, loginPage.Cookies, true, referer: SiteLink);
            await ConfigureIfOK(result.Cookies, result.Content != null && result.Content.Contains("Főoldal"), () =>
            {
                throw new ExceptionWithConfigData("Error while trying to login with: Username: " + configData.Username.Value +
                                                  " Password: " + configData.Password.Value, configData);
            });

            return IndexerConfigurationStatus.RequiresTesting;
        }

        /// <summary>
        /// Parses the torrents drom the contetn
        /// </summary>
        /// <returns>The parsed torrents.</returns>
        /// <param name="results">The result of the querry</param>
        /// <param name="query">Query.</param>
        /// <param name="already_founded">Number of the already founded torrents.(used for limit)</param>
        /// <param name="limit">The limit to the number of torrents to download </param>
=======
            var result =   await RequestLoginAndFollowRedirect(LoginUrl, pairs, loginPage.Cookies, true, referer: SiteLink);
            await ConfigureIfOK(result.Cookies, result.Content != null && result.Content.Contains("Főoldal"), () =>
            {
                throw new ExceptionWithConfigData("Error while trying to login with: Username: "+ configData.Username.Value +
                                                  " Password: "+ configData.Password.Value, configData);
            });


           

            return IndexerConfigurationStatus.RequiresTesting;
        }

>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
        async Task<List<ReleaseInfo>> ParseTorrents(WebClientStringResult results, TorznabQuery query, int already_founded, int limit)
        {
            var releases = new List<ReleaseInfo>();
            try
            {
                String content = results.Content;
<<<<<<< HEAD
                /* Content Looks like this
                 * 2\15\2\1\1727\207244\1x08 \[WebDL-720p - Eng - AJP69]\gb\2018-03-09 08:11:53\akció, kaland, sci-fi \0\0\1\191170047\1\0\Anonymous\50\0\0\\0\4\0\174\0\
                 * 1\ 0\0\1\1727\207243\1x08 \[WebDL-1080p - Eng - AJP69]\gb\2018-03-09 08:11:49\akció, kaland, sci-fi \0\0\1\305729738\1\0\Anonymous\50\0\0\\0\8\0\102\0\0\0\0\1\\\*/
=======
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                var splits = content.Split('\\');
                int i = 0;

                ReleaseInfo release = new ReleaseInfo();
<<<<<<< HEAD

                /* Split the relases by '\' and go throw them. 
                 * 26 element belongs to one torrent*/
=======
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                foreach (var s in splits)
                {
                    switch (i)
                    {
                        case 4:
                            //ID of the series
                            //Get IMDB id form site series database
                            SeriesDetail seriesinfo = series.Find(x => x.id.Contains(s));
                            if (seriesinfo != null && !s.Equals(""))
                                release.Imdb = long.Parse(seriesinfo.imdbid);
                            goto default;
                        case 5:
                            //ID of the torrent
                            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                            string fileinfoURL = "https://tvstore.me/torrent/br_process.php?func=getToggle&id=" + s + "&w=F&pg=0&now=" + unixTimestamp;
                            string fileinfo = (await RequestStringWithCookiesAndRetry(fileinfoURL)).Content;
                            release.Link = new Uri(SiteLink.ToString() + "torrent/download.php?id=" + s);
                            string[] fileinf = fileinfo.Split(new string[] { "\\\\" }, StringSplitOptions.None);
<<<<<<< HEAD
                            if (fileinf.Length > 1)
=======
                            if (fileinf.Length>1)
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                                release.Title = fileinf[1];
                            goto default;
                        /*case 6:
                            Console.WriteLine("Series season/ep =" + s); --> 9x10
                            goto default;*/
                        /*case 7:
                            Console.WriteLine("Releaseinfo =" + s);  --->Releaseinfo =[HDTV - Rip - Eng - SVA]
                            goto default;*/
                        case 9:
                            release.PublishDate = DateTime.Parse(s, CultureInfo.InvariantCulture);
                            goto default;
                        case 13:
                            release.Files = int.Parse(s);
                            goto default;
                        case 14:
                            release.Size = long.Parse(s);
                            goto default;
                        case 23:
                            release.Seeders = int.Parse(s);
                            goto default;
                        case 24:
                            release.Peers = (int.Parse(s) + release.Seeders);
                            goto default;
                        case 25:
                            release.Grabs = int.Parse(s);
                            goto default;
                        case 26:
<<<<<<< HEAD
                            /*This is the last element for the torrent. So add it to releases and start parsing to new torrent*/
                            i = 0;
                            release.Category = new List<int> { TvCategoryParser.ParseTvShowQuality(release.Title) };
                            //todo Added some basic configuration need to improve it
=======

                            i = 0;
                            release.Category = new List<int> { TvCategoryParser.ParseTvShowQuality(release.Title) };
                            // Added some basic configuration need to improve it
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                            release.MinimumRatio = 1;
                            release.MinimumSeedTime = 172800;
                            release.DownloadVolumeFactor = 1;
                            release.UploadVolumeFactor = 1;

                            if ((already_founded + releases.Count) < limit)
<<<<<<< HEAD
                            {
                                releases.Add(release);
                            }
                            else
                            {
                                return releases;
                            }
=======
                                releases.Add(release);
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                            release = new ReleaseInfo();
                            break;
                        default:
                            i++;
                            break;
                    }
<<<<<<< HEAD

=======
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                }

            }
            catch (Exception ex)
            {
                OnParseError(results.Content, ex);
            }

            return releases;
        }
        /* Search is possible only based by Series ID. 
         * All known series ID is on main page, with their attributes. (ID, EngName, HunName, imdbid)*/
<<<<<<< HEAD

        /// <summary>
        /// Get all series info known by site
        /// These are:
        ///     - Series ID
        ///     - Hungarian name
        ///     - English name
        ///     - IMDB ID
        /// </summary>
        /// <returns>The series info.</returns>
=======
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
        protected async Task<Boolean> GetSeriesInfo()
        {

            var result = (await RequestStringWithCookiesAndRetry("https://tvstore.me/torrent/browse.php")).Content;

            CQ dom = result;
            var scripts = dom["script"];

            foreach (var script in scripts)
            {
                if (script.TextContent.Contains("catsh=Array"))
                {
                    string[] seriesknowbysite = Regex.Split(script.TextContent, "catl");
                    for (int i = 1; i < seriesknowbysite.Length; i++)
                    {
                        try
                        {
                            var id = seriesknowbysite[i];
                            string[] serieselement = id.Split(';');
                            SeriesDetail sd = new SeriesDetail();
                            sd.HunName = serieselement[1].Split('=')[1].Trim('\'').ToLower();
                            sd.EngName = serieselement[2].Split('=')[1].Trim('\'').ToLower();
                            sd.id = serieselement[0].Split('=')[1].Trim('\'');
                            sd.imdbid = serieselement[7].Split('=')[1].Trim('\'');
                            series.Add(sd);
                        }
                        catch (IndexOutOfRangeException e)
                        { }
<<<<<<< HEAD
=======
                       
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
                    }
                }
            }
            return true;
        }

        protected override async Task<IEnumerable<ReleaseInfo>> PerformQuery(TorznabQuery query)
        {
            var releases = new List<ReleaseInfo>();

<<<<<<< HEAD
            /* If series from sites are indexed than we dont need to reindex them.*/
=======
            /* If series from sites are indexed than we dont need it.*/
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
            if (series == null || series.IsEmpty())
            {
                await GetSeriesInfo();
            }
<<<<<<< HEAD

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            WebClientStringResult results;

            string searchString = "";
            /* SearcString format is the following: Seriesname 1X09*/
            if (!query.SearchTerm.Equals(""))
            {
                searchString += query.SanitizedSearchTerm;
                if (query.Season != 0)
                    searchString += " " + query.Season.ToString();
                if (query.Episode != null && !query.Episode.Equals(""))
                    searchString += string.Format("x{0:00}", int.Parse(query.Episode));
            }

            /* Search string must be converted to Base64 */
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(searchString);
            var base64coded = System.Convert.ToBase64String(plainTextBytes);

            /*Start search*/
            int page = 1;
            string exactSearchURL = SearchUrl + "?gyors=" + base64coded +"&p="+ page +"&now=" + unixTimestamp.ToString();
            results = await RequestStringWithCookiesAndRetry(exactSearchURL);

            /* Parse page Information from result*/
            string content = results.Content;
            var splits = content.Split('\\');
            int maxfounded = int.Parse(splits[0]);
            int perpage = int.Parse(splits[1]);
            //int incurrentpage = int.Parse(splits[2]);
            double pages = Math.Ceiling((double)maxfounded / (double)perpage);


            var limit = query.Limit;
            if (limit == 0)
                limit = 100;
            /* First page content is already ready*/
            releases.AddRange(await ParseTorrents(results, query, releases.Count, limit));

            for (page =2;(page<=pages && releases.Count<limit);page++)
            {
                exactSearchURL = SearchUrl + "?gyors=" + base64coded + "&p=" + page + "&now=" + unixTimestamp.ToString();
                results = await RequestStringWithCookiesAndRetry(exactSearchURL);
                releases.AddRange(await ParseTorrents(results, query, releases.Count, limit));

            }

=======
            if (query.SearchTerm == null)
                query.SearchTerm = "";

            /*todo if not exact name is comming in SearchTerm will wont work.     
            /* Search for series based on English name in the list parsed before */
            SeriesDetail seriesinfo = series.Find(x => x.EngName.Contains(query.SearchTerm.ToLower()));
            if (seriesinfo == null)
            {
                /* Search for series based on Hungarian name if English has no match*/
                seriesinfo = series.Find(x => x.HunName.Contains(query.SearchTerm.ToLower()));
            }
            if (seriesinfo == null)
                return releases;

            Console.WriteLine("AAAA Season " + seriesinfo.id);

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            /*var pairs = new List<KeyValuePair<string, string>>();
            pairs.Add(new KeyValuePair<string, string>("g", seriesinfo.id));
            pairs.Add(new KeyValuePair<string, string>("s", query.Season.ToString()));
            pairs.Add(new KeyValuePair<string, string>("e", query.Episode));
            pairs.Add(new KeyValuePair<string, string>("now", unixTimestamp.ToString()));
            var results = await PostDataWithCookiesAndRetry(SearchUrl, pairs);
            */

            WebClientStringResult results;

            /* Create the search URL. 
             * It will be better to use some func from Baseindexer but they wont work
               Links looks the following https://tvstore.me/torrent/br_process.php?s=1&e=2&g=33&now=1550570305277
             */
            string exactSearchURL = SearchUrl;
            if (!query.SearchTerm.Equals(""))
            {
                exactSearchURL += "?g=" + seriesinfo.id;
                if (query.Season != 0)
                    exactSearchURL += "&s=" + query.Season.ToString();
                if (query.Episode != null && !query.Episode.Equals(""))
                    exactSearchURL += "&e=" + query.Episode;
                exactSearchURL += "&now=" + unixTimestamp.ToString();
            }

            results = await RequestStringWithCookiesAndRetry(exactSearchURL);

            var limit = query.Limit;
            if (limit == 0)
                limit = 100;

            releases = await ParseTorrents(results, query, releases.Count, limit);
              
>>>>>>> b093c461260eaade262f92f716620d14e0b5da11
            return releases;
        }
    }
    public class SeriesDetail
    {
        public string id;
        public string HunName;
        public string EngName;
        public string imdbid;

    }

}
