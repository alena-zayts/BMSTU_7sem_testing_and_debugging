using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Models;

namespace BL.Tests.ArrangeHelpers
{
    public class UsersObjectMother
    {

        public static User UnauthorizedUser()
        {
            return new User(1, 0, "", "", PermissionsEnum.UNAUTHORIZED);
        }

        public static User AuthorizedUser()
        {
            return new User(2, 2, "authorized_user_email", "authorized_user_password", PermissionsEnum.AUTHORIZED);
        }

        public static User SkiPatrolUser()
        {
            return new User(3, 3, "skipatrol_user_email", "skipatrol_user_password", PermissionsEnum.SKI_PATROL);
        }

        public static User AdminUser()
        {
            return new User(4, 1, "admin_user_email", "admin_user_password", PermissionsEnum.ADMIN);
        }

    }
}
