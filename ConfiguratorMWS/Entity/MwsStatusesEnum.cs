namespace ConfiguratorMWS.Entity
{
    public enum MwsStatusesEnum
    {
        Command71Accepted,
        Command80Accepted,
        Command85Accepted,
        Command90Accepted,
        Command90AcceptedAndTimerIntervalChanged,
        Command91Accepted,
        Command92Accepted,
        DeviceFlashClear,
        DeviceFlashWrite,
        DeviceReset,

        //update Firmware
        DeviceUpdateRequestReset,
        DeviceUpdateRequestBootInfo,
        DviceUpdateRequestEraseFlash,
        DeviceUpdateResponsData,

        //reset settings
        DeviceSetDefaultSettings,
        DeviceResetAfterSettingDefaultSettings
    }
}
