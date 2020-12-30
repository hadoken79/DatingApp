namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }//EntityFramework conform
        public string UserName { get; set; }//AspNetCoreIdentity conform 
    }
}