using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Il2CppSLZ.Marrow;

namespace NEP.MagPerception.Helper
{
    public static class GripHelper
    {
        public static List<Grip> GetGrips(this Gun gun)
        {
            if (gun == null)
                return [];

            List<Grip> grips = [gun.triggerGrip];
            grips.AddRange(gun.slideVirtualController.primaryGrips);
            grips.AddRange(gun.otherGrips);
            return grips;
        }
    }
}