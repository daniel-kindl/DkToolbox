using DkToolbox.Core.Models;

namespace DkToolbox.Core.Abstractions;

public interface IPortService
{
    IReadOnlyList<PortOwnerInfo> WhoOwns(PortQuery query);
}