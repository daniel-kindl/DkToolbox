using DkToolbox.Core.Abstractions;
using DkToolbox.Core.Models;

namespace DkToolbox.Platform.Windows;

public sealed class WindowsPortService : IPortService
{
    public IReadOnlyList<PortOwnerInfo> WhoOwns(PortQuery query)
    {
        throw new NotImplementedException();
    }
}
