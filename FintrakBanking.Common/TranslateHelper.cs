using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FintrakBanking.Common
{
    public static class TranslateHelper
    {
        public static string get(string word)
        {
            var lang = HttpContext.Current.Request.Headers["X-LANG"];
            if(lang == null)
            {
                lang = "en";
            }
            string result = word;
            
            switch (lang)
            {
                case "fr":
                    var dic = frenchDics();
                    var translation = "";
                   if (dic.TryGetValue(word, out translation)) {
                        result = translation;
                    }
                    else
                    {

                    }
     
                    break;

            }
            return result;
        }

        public static Dictionary<string, string> frenchDics()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("word1", "translation");
          

            return dictionary;
        }
    }

     
}
