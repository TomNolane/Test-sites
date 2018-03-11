using System.Collections.Generic;

namespace TestSites
{
    public class Errors
    {
        List<Error> eror_list = new List<Error>();

        public List<Error> GetList()
        {
            return eror_list;
        }

        public void AddList(Error _error)
        {
            eror_list.Add(_error);
        }
    }

    public class Error
    {
        public string SiteName { get; set; } = "";
        public string ErrorName { get; set; } = "";
        public string UrlName { get; set; } = "";
    }
}
