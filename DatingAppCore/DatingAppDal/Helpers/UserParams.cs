namespace DatingAppWebApi.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize= 50;
        public int PageNumber { get; set; }
        private int PageSize;
        public int pageSize
        {
            get { return PageSize;}
            set { PageSize= value>MaxPageSize?MaxPageSize:value;}
        }

        public string Gender{get;set;}
        public int UserId{get;set;}
        public int MaxAge{get;set;} = 99;
        public int MinAge{get;set;} = 18;
        public string OrderBy { get; set; }

        public bool Likers{get;set;}
        public bool Likees{get;set;}
        
    }
}