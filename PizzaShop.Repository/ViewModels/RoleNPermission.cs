namespace pizzashop.Repository.ViewModels
{
    public partial class RoleNPermission
    {

        public int Roleid { get; set; }

        public string Rolename { get; set; } = null!;

        public List<Permissions>? Permissions { get; set; }
    }
    public class Permissions
    {
        public int Permissionid { get; set; }

        public int Moduleid { get; set; }

        public string Modulename { get; set; } = null!;

        public bool Canview { get; set; }

        public bool Canaddedit { get; set; }

        public bool Candelete { get; set; }
    }
}

