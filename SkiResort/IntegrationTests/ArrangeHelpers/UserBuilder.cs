using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;


namespace IntegrationTests.ArrangeHelpers
{
    public class UserBuilder
    {

        public static string DEFAULT_EMAIL = "default_email";
        public static string DEFAULT_PASSWORD = "default_password";
        public static PermissionsEnum DEFAULT_PERMISSIONS = PermissionsEnum.UNAUTHORIZED;
        public static uint DEFAULT_CARD_ID = 0;
        public static uint DEFAULT_ID = 0;

        private string email = DEFAULT_EMAIL;
        private string password = DEFAULT_PASSWORD;
        private PermissionsEnum permissions = DEFAULT_PERMISSIONS;
        private uint cardID = DEFAULT_CARD_ID;
        private uint id = DEFAULT_ID;

        private UserBuilder()
        {
        }

        public static UserBuilder aUser()
        {
            return new UserBuilder();
        }

        public UserBuilder withEmail(string email)
        {
            this.email = email;
            return this;
        }
        public UserBuilder withoutEmail()
        {
            this.email = "";
            return this;
        }

        public UserBuilder withPassword(string password)
        {
            this.password = password;
            return this;
        }
        public UserBuilder withoutPassword()
        {
            this.password = "";
            return this;
        }

        public UserBuilder inUnauthorizedRole()
        {
            this.permissions = PermissionsEnum.UNAUTHORIZED;
            return this;
        }

        public UserBuilder inAuthorizedRole()
        {
            this.permissions = PermissionsEnum.AUTHORIZED;
            return this;
        }

        public UserBuilder inSkiPatrolRole()
        {
            this.permissions = PermissionsEnum.SKI_PATROL;
            return this;
        }

        public UserBuilder inAdminRole()
        {
            this.permissions = PermissionsEnum.ADMIN;
            return this;
        }

        public UserBuilder inRole(PermissionsEnum permissions)
        {
            this.permissions = permissions;
            return this;
        }

        public UserBuilder but()
        {
            return UserBuilder
                    .aUser();
        }

        public User build()
        {
            return new User(id, cardID, email, password, permissions);
        }
    }
}
