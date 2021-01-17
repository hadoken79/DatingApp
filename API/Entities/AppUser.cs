namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }//EntityFramework conform
        public string UserName { get; set; }//AspNetCoreIdentity conform 

        public byte[] PasswordHash {get; set;}
        public byte[] PasswordSalt { get; set; }
    }
}