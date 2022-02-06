using System.Collections;
using UnityEngine;

namespace ProjectGuardian
{
    public interface IPortal
    {
        public bool isPortalOpen { get; }
        public void OpenPortal();
        public void ClosePortal();
    }
}