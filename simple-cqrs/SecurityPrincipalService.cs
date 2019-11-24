using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCQRS
{
   public interface ISecurityPrincipalService
   {
      string ActiveUserName();
   }
}
