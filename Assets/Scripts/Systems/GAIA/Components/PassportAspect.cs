using DreamersIncStudio.GAIACollective;
using Unity.Entities;
using UnityEngine;

namespace DreamersIncStudio.GAIACollective
{
    public readonly partial struct PassportAspect : IAspect
    {
        private readonly RefRO<GaiaLife> life;
    }
}