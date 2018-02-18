using System;

namespace SaluteOnline.Shared.Common
{
    public interface IEntity
    {
        Guid Guid { get; set; }
        int Id { get; set; }
    }
}