using ICities;
using UnityEngine;

using System;

using ColossalFramework;
using ColossalFramework.UI;

using FineRoadTool.TranslationFramework;

/*using System.Linq;
using ColossalFramework.Plugins;
using ColossalFramework.PlatformServices;*/

namespace FineRoadTool
{
    public class FineRoadTool : IUserMod
    {
       

        public FineRoadTool()
        {
            try
            {
                // Creating setting file
                GameSettings.AddSettingsFile(new SettingsFile[] { new SettingsFile() { fileName = FineRoadTooLoader.settingsFileName } });
            }
            catch (Exception e)
            {
                DebugUtils.Log("Could load/create the setting file.");
                DebugUtils.LogException(e);
            }
        }

        public const string version = "1.3.2";

        public static Translation translation = new Translation();

        public string Name => $"Fine Road Tool [r{version}]";
        public string Description => translation.GetTranslation("FRT_DESCRIPTION");
        

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                UIHelper group = helper.AddGroup(Name) as UIHelper;
                UIPanel panel = group.self as UIPanel;

                UICheckBox checkBox = (UICheckBox)group.AddCheckbox(translation.GetTranslation("FRT_OPT_REDUCE_CATENARY_MAST_LABEL"), FineRoadTooLoader.reduceCatenary.value, (b) =>
                {
                    FineRoadTooLoader.reduceCatenary.value = b;
                    if (FineRoadTooLoader.instance != null)
                    {
                        FineRoadTooLoader.instance.UpdateCatenary();
                    }
                });
                checkBox.tooltip = translation.GetTranslation("FRT_OPT_REDUCE_CATENARY_MAST_TOOLTIP");

                group.AddSpace(10);

                panel.gameObject.AddComponent<OptionsKeymapping>();

                group.AddSpace(10);

                group.AddButton(translation.GetTranslation("FRT_OPT_RESET_BUTTON"), () =>
                {
                    UIToolOptionsButton.savedWindowX.Delete();
                    UIToolOptionsButton.savedWindowY.Delete();

                    if (UIToolOptionsButton.toolOptionsPanel)
                        UIToolOptionsButton.toolOptionsPanel.absolutePosition = new Vector3(-1000, -1000);
                });

                /*PublishedFileId SJA = new PublishedFileId(553184329);
                if (PlatformService.active && PlatformService.workshop.GetSubscribedItems().Contains(SJA))
                {
                    PlatformService.workshop.Unsubscribe(SJA);

                    PublishedFileId FRA = new PublishedFileId(802066100);
                    Workshop.WorkshopItemInstalledHandler workshopItemInstalled = null;
                    workshopItemInstalled = (id) =>
                     {
                         if (id == FRA)
                         {
                             foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
                             {
                                 if (plugin.publishedFileID == FRA)
                                 {
                                     plugin.isEnabled = true;
                                     PlatformService.workshop.eventWorkshopItemInstalled -= workshopItemInstalled;
                                 }
                             }
                         }
                     };

                    PlatformService.workshop.eventWorkshopItemInstalled += workshopItemInstalled;
                    PlatformService.workshop.Subscribe(FRA);
                }*/
            }
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
                DebugUtils.LogException(e);
            }
        }

       
    }
}
