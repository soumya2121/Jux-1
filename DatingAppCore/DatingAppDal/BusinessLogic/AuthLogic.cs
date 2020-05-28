namespace DatingAppDal.BusinessLogic
{
    public class AuthLogic
    {
        public static void CreatePasswordHash(string password,out byte[] passwordhash,out byte[] passwordsalt)
        {
            using (var hmac= new System.Security.Cryptography.HMACSHA512())
            {
               passwordsalt= hmac.Key;
               passwordhash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

         public static bool VerifyPasswordHash(string password, byte[] passwordhash,byte[] passwordsalt)
        {
            using (var hmac= new System.Security.Cryptography.HMACSHA512(passwordsalt))
            {
              var generatepasswordhash= hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
              for(int i=0;i<generatepasswordhash.Length;i++)
              {
                  if(passwordhash[i]!=generatepasswordhash[i])
                  return false;
              }
              return  true;
            }
        }
    }
}