// ReSharper disable once CheckNamespace
namespace Easy.Common
{
    /// <summary>
    /// Represents the details of the drives installed in the system.
    /// </summary>
    public sealed class DriveDetails
    {
        /// <summary>
        /// Gets the name of the drive.
        /// </summary>
        public string Name { get; internal set; }
        
        /// <summary>
        /// Gets the type of the drive e.g. <c>Fixed, CDRom</c> etc.
        /// </summary>
        public string Type { get; internal set; }
        
        /// <summary>
        /// Gets the format of the drive e.g. <c>NTFS</c>.
        /// </summary>
        public string Format { get; internal set; }
        
        /// <summary>
        /// Gets the label of the drive.
        /// </summary>
        public string Label { get; internal set; }
        
        /// <summary>
        /// Gets the capacity of the drive.
        /// </summary>
        public double TotalCapacityInGigaBytes { get; internal set; }
        
        /// <summary>
        /// Gets the total amount of free space available on the drive 
        /// (not just what is available to the current user).
        /// </summary>
        public double FreeCapacityInGigaBytes { get; internal set; }
        
        /// <summary>
        /// Gets the amount of free space available on the drive.
        /// <remarks>This value takes into account disk quotas.</remarks>
        /// </summary>
        public double AvailableCapacityInGigaBytes { get; internal set; }
    }
}