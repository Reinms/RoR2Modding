﻿using System.Security;
using System.Security.Permissions;


#region Module



[module: UnverifiableCode]



#endregion





#region Assembly



#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission( SecurityAction.RequestMinimum, SkipVerification = true )]
#pragma warning restore CS0618 // Type or member is obsolete



#endregion