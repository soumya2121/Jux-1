using System;

namespace DatingAppDal.Helpers
{
    public static class Extensions
    {
         public static int CalculateAge(this DateTime datetime)
        {
            int age= DateTime.Now.Year - datetime.Year;
            if(datetime.AddYears(age) > DateTime.Now)
            {
               age--;
            }
            return age;
        }
    }
}