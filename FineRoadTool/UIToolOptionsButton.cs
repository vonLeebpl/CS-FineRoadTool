﻿using System;
using System.Reflection;

using UnityEngine;

using ColossalFramework;
using ColossalFramework.UI;

namespace FineRoadTool
{
    public class UIToolOptionsButton : UICheckBox
    {
        private UIButton m_button;
        private UIPanel m_toolOptionsPanel;
        private UISlider m_elevationStepSlider;
        private UILabel m_elevationStepLabel;

        private UICheckBox m_normalModeButton;
        private UICheckBox m_groundModeButton;
        private UICheckBox m_elevatedModeButton;
        private UICheckBox m_bridgeModeButton;
        private UICheckBox m_tunnelModeButton;

        private UICheckBox m_straightSlope;

        private UITextureAtlas m_atlas;

        private UIComponent m_parent;

        public static readonly SavedInt savedWindowX = new SavedInt("windowX", FineRoadTooLoader.settingsFileName, -1000, true);
        public static readonly SavedInt savedWindowY = new SavedInt("windowY", FineRoadTooLoader.settingsFileName, -1000, true);

        public static readonly SavedBool windowVisible = new SavedBool("windowVisible", FineRoadTooLoader.settingsFileName, false, true);

        public static UIPanel toolOptionsPanel = null;

        public override void Start()
        {
            LoadResources();

            CreateButton();
            CreateOptionPanel();

            isChecked = windowVisible;
        }

        public override void Update()
        {
            if (parent != m_parent && parent != null)
            {
                m_parent = parent;

                UpdateInfo();
                DebugUtils.Log("Tool button parent changed: " + parent.name);
            }

            if (m_toolOptionsPanel != null)
            {
                m_toolOptionsPanel.isVisible = isVisible && isChecked;
            }
        }

        public void UpdateInfo()
        {
            if (FineRoadTooLoader.instance == null) return;

            if (parent != null)
            {
                if (parent.name == "OptionsBar")
                {
                    relativePosition = new Vector2(36, 0);
                }
                else
                {
                    relativePosition = Vector2.zero;
                    parent.BringToFront();
                }
            }
            else
            {
                isVisible = false;
                return;
            }

            m_button.text = m_elevationStepLabel.text = FineRoadTooLoader.instance.elevationStep + FineRoadTool.translation.GetTranslation("FRT_OPBTN_METERS") + "\n";
            m_elevationStepSlider.value = FineRoadTooLoader.instance.elevationStep;
            m_straightSlope.isChecked = FineRoadTooLoader.instance.straightSlope;

            m_button.normalFgSprite = FineRoadTooLoader.instance.straightSlope ? "ToolbarIconGroup1Hovered" : null;

            switch (FineRoadTooLoader.instance.mode)
            {
                case Mode.Normal:
                    m_button.text += FineRoadTool.translation.GetTranslation("FRT_OPBTN_NORMAL") + "\n";
                    m_normalModeButton.SimulateClick();
                    break;
                case Mode.Ground:
                    m_button.text += FineRoadTool.translation.GetTranslation("FRT_OPBTN_GROUND") + "\n"; ;
                    m_groundModeButton.SimulateClick();
                    break;
                case Mode.Elevated:
                    m_button.text += FineRoadTool.translation.GetTranslation("FRT_OPBTN_ELEVATED") + "\n"; ;
                    m_elevatedModeButton.SimulateClick();
                    break;
                case Mode.Bridge:
                    m_button.text += FineRoadTool.translation.GetTranslation("FRT_OPBTN_BRIDGE") + "\n"; ;
                    m_bridgeModeButton.SimulateClick();
                    break;
                case Mode.Tunnel:
                    m_button.text += FineRoadTool.translation.GetTranslation("FRT_OPBTN_TUNEL") + "\n"; ;
                    m_tunnelModeButton.SimulateClick();
                    break;
            }

            m_button.text += FineRoadTooLoader.instance.elevation + FineRoadTool.translation.GetTranslation("FRT_OPBTN_METERS") + "\n"; ;
        }

        private void CreateButton()
        {
            m_button = AddUIComponent<UIButton>();
            m_button.atlas = ResourceLoader.GetAtlas("Ingame");
            m_button.name = "FRT_MainButton";
            m_button.size = new Vector2(36, 36);
            m_button.textScale = 0.7f;
            m_button.playAudioEvents = true;
            m_button.relativePosition = Vector2.zero;

            m_button.tooltip = "Fine Road Tool " + FineRoadTool.version + "\n\n" + FineRoadTool.translation.GetTranslation("FRT_OPBTN_TOOLTIP");

            m_button.textColor = Color.white;
            m_button.textScale = 0.7f;
            m_button.dropShadowOffset = new Vector2(2, -2);
            m_button.useDropShadow = true;

            m_button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            m_button.wordWrap = true;

            m_button.normalBgSprite = "OptionBase";
            m_button.hoveredBgSprite = "OptionBaseHovered";
            m_button.pressedBgSprite = "OptionBasePressed";
            m_button.disabledBgSprite = "OptionBaseDisabled";

            m_button.eventClick += (c, p) => { base.OnClick(p); };

            eventCheckChanged += (c, isChecked) =>
            {
                if (isChecked)
                {
                    if (m_toolOptionsPanel.absolutePosition.x < 0)
                    {
                        m_toolOptionsPanel.absolutePosition = new Vector2(absolutePosition.x - (m_toolOptionsPanel.width - m_button.width) / 2, absolutePosition.y - m_toolOptionsPanel.height);
                    }

                    Vector2 resolution = GetUIView().GetScreenResolution();

                    m_toolOptionsPanel.absolutePosition = new Vector2(
                        Mathf.Clamp(m_toolOptionsPanel.absolutePosition.x, 0, resolution.x - m_toolOptionsPanel.width),
                        Mathf.Clamp(m_toolOptionsPanel.absolutePosition.y, 0, resolution.y - m_toolOptionsPanel.height));

                    m_button.normalBgSprite = "OptionBaseFocused";

                    m_toolOptionsPanel.BringToFront();
                    UpdateInfo();
                }
                else
                {
                    m_button.normalBgSprite = "OptionBase";
                }
                UpdateInfo();
            };
        }

        protected override void OnClick(UIMouseEventParameter p) { }

        private void CreateOptionPanel()
        {
            m_toolOptionsPanel = UIView.GetAView().AddUIComponent(typeof(UIPanel)) as UIPanel;
            m_toolOptionsPanel.name = "FRT_ToolOptionsPanel";
            m_toolOptionsPanel.atlas = ResourceLoader.GetAtlas("Ingame");
            m_toolOptionsPanel.backgroundSprite = "SubcategoriesPanel";
            m_toolOptionsPanel.size = new Vector2(228, 180);
            m_toolOptionsPanel.absolutePosition = new Vector3(savedWindowX.value, savedWindowY.value);
            m_toolOptionsPanel.clipChildren = true;

            m_toolOptionsPanel.isVisible = false;

            DebugUtils.Log("absolutePosition: " + m_toolOptionsPanel.absolutePosition);

            m_toolOptionsPanel.eventPositionChanged += (c, p) =>
            {
                if (m_toolOptionsPanel.absolutePosition.x < 0)
                    m_toolOptionsPanel.absolutePosition = new Vector2(absolutePosition.x - (m_toolOptionsPanel.width - m_button.width) / 2, absolutePosition.y - m_toolOptionsPanel.height);

                Vector2 resolution = GetUIView().GetScreenResolution();

                m_toolOptionsPanel.absolutePosition = new Vector2(
                    Mathf.Clamp(m_toolOptionsPanel.absolutePosition.x, 0, resolution.x - m_toolOptionsPanel.width),
                    Mathf.Clamp(m_toolOptionsPanel.absolutePosition.y, 0, resolution.y - m_toolOptionsPanel.height));

                savedWindowX.value = (int)m_toolOptionsPanel.absolutePosition.x;
                savedWindowY.value = (int)m_toolOptionsPanel.absolutePosition.y;
            };

            toolOptionsPanel = m_toolOptionsPanel;

            UIDragHandle dragHandle = m_toolOptionsPanel.AddUIComponent<UIDragHandle>();
            dragHandle.size = m_toolOptionsPanel.size;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = parent;

            // Elevation step
            UILabel label = m_toolOptionsPanel.AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = FineRoadTool.translation.GetTranslation("FRT_OPBTN_ELEVATION_STEP");
            label.relativePosition = new Vector2(8, 8);
            label.SendToBack();

            UIPanel sliderPanel = m_toolOptionsPanel.AddUIComponent<UIPanel>();
            sliderPanel.atlas = m_toolOptionsPanel.atlas;
            sliderPanel.backgroundSprite = "GenericPanel";
            sliderPanel.color = new Color32(206, 206, 206, 255);
            sliderPanel.size = new Vector2(m_toolOptionsPanel.width - 16, 36);
            sliderPanel.relativePosition = new Vector2(8, 28);

            m_elevationStepLabel = sliderPanel.AddUIComponent<UILabel>();
            m_elevationStepLabel.atlas = m_toolOptionsPanel.atlas;
            m_elevationStepLabel.backgroundSprite = "TextFieldPanel";
            m_elevationStepLabel.verticalAlignment = UIVerticalAlignment.Bottom;
            m_elevationStepLabel.textAlignment = UIHorizontalAlignment.Center;
            m_elevationStepLabel.textScale = 0.7f;
            m_elevationStepLabel.text = "3m";
            m_elevationStepLabel.autoSize = false;
            m_elevationStepLabel.color = new Color32(91, 97, 106, 255);
            m_elevationStepLabel.size = new Vector2(38, 15);
            m_elevationStepLabel.relativePosition = new Vector2(sliderPanel.width - m_elevationStepLabel.width - 8, 10);

            m_elevationStepSlider = sliderPanel.AddUIComponent<UISlider>();
            m_elevationStepSlider.name = "FRT_ElevationStepSlider";
            m_elevationStepSlider.size = new Vector2(sliderPanel.width - 20 - m_elevationStepLabel.width - 8, 18);
            m_elevationStepSlider.relativePosition = new Vector2(10, 10);


            // m_elevationStepSlider.tooltip = OptionsKeymapping.elevationStepUp.ToLocalizedString("KEYNAME") + " and " + OptionsKeymapping.elevationStepDown.ToLocalizedString("KEYNAME") + " to change Elevation Step";
            m_elevationStepSlider.tooltip = string.Format(
                FineRoadTool.translation.GetTranslation("FRT_OPBTN_ELEVATION_STEP_SLIDER_TOOLTIP"),
                OptionsKeymapping.elevationStepUp.ToLocalizedString("KEYNAME"),
                OptionsKeymapping.elevationStepDown.ToLocalizedString("KEYNAME")
                );

            UISlicedSprite bgSlider = m_elevationStepSlider.AddUIComponent<UISlicedSprite>();
            bgSlider.atlas = m_toolOptionsPanel.atlas;
            bgSlider.spriteName = "BudgetSlider";
            bgSlider.size = new Vector2(m_elevationStepSlider.width, 9);
            bgSlider.relativePosition = new Vector2(0, 4);

            UISlicedSprite thumb = m_elevationStepSlider.AddUIComponent<UISlicedSprite>();
            thumb.atlas = m_toolOptionsPanel.atlas;
            thumb.spriteName = "SliderBudget";
            m_elevationStepSlider.thumbObject = thumb;

            m_elevationStepSlider.stepSize = 1;
            m_elevationStepSlider.minValue = 1;
            m_elevationStepSlider.maxValue = 12;
            m_elevationStepSlider.value = 3;

            m_elevationStepSlider.eventValueChanged += (c, v) =>
            {
                if (v != FineRoadTooLoader.instance.elevationStep)
                {
                    FineRoadTooLoader.instance.elevationStep = (int)v;
                    UpdateInfo();
                }
            };

            // Modes
            label = m_toolOptionsPanel.AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES");
            label.relativePosition = new Vector2(8, 72);
            label.SendToBack();

            UIPanel modePanel = m_toolOptionsPanel.AddUIComponent<UIPanel>();
            modePanel.atlas = m_toolOptionsPanel.atlas;
            modePanel.backgroundSprite = "GenericPanel";
            modePanel.color = new Color32(206, 206, 206, 255);
            modePanel.size = new Vector2(m_toolOptionsPanel.width - 16, 52);
            modePanel.relativePosition = new Vector2(8, 92);

            modePanel.padding = new RectOffset(8, 8, 8, 8);
            modePanel.autoLayoutPadding = new RectOffset(0, 4, 0, 0);
            modePanel.autoLayoutDirection = LayoutDirection.Horizontal;

            m_normalModeButton = CreateModeCheckBox(modePanel, "NormalMode", FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES_NORMAL"));
            m_groundModeButton = CreateModeCheckBox(modePanel, "GroundMode", FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES_GROUND"));
            m_elevatedModeButton = CreateModeCheckBox(modePanel, "ElevatedMode", FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES_ELEVATED"));
            m_bridgeModeButton = CreateModeCheckBox(modePanel, "BridgeMode", FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES_BRIDGE"));
            m_tunnelModeButton = CreateModeCheckBox(modePanel, "TunnelMode", FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES_TUNNEL"));

            modePanel.autoLayout = true;

            // Straight Slope
            m_straightSlope = CreateCheckBox(m_toolOptionsPanel);
            m_straightSlope.name = "FRT_StraightSlope";
            m_straightSlope.label.text = FineRoadTool.translation.GetTranslation("FRT_OPBTN_STRIGHT_SLOPE_LABEL");
            m_straightSlope.tooltip = FineRoadTool.translation.GetTranslation("FRT_OPBTN_STRIGHT_SLOPE_TOOLTIP1") + "\n\n" +
                OptionsKeymapping.toggleStraightSlope.ToLocalizedString("KEYNAME") +
                FineRoadTool.translation.GetTranslation("FRT_OPBTN_STRIGHT_SLOPE_TOOLTIP2");
            m_straightSlope.isChecked = false;
            m_straightSlope.relativePosition = new Vector3(8, 152);

            m_straightSlope.eventCheckChanged += (c, state) =>
            {
                FineRoadTooLoader.instance.straightSlope = state;
            };

            m_normalModeButton.isChecked = true;
        }

        private UICheckBox CreateCheckBox(UIComponent parent)
        {
            UICheckBox checkBox = (UICheckBox)parent.AddUIComponent<UICheckBox>();

            checkBox.width = 300f;
            checkBox.height = 20f;
            checkBox.clipChildren = true;

            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.atlas = m_toolOptionsPanel.atlas;
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = Vector3.zero;

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).atlas = m_toolOptionsPanel.atlas;
            ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.label.text = " ";
            checkBox.label.textScale = 0.9f;
            checkBox.label.relativePosition = new Vector3(22f, 2f);

            return checkBox;
        }

        private UICheckBox CreateModeCheckBox(UIComponent parent, string spriteName, string toolTip)
        {
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.size = new Vector2(36, 36);
            checkBox.group = parent;

            UIButton button = checkBox.AddUIComponent<UIButton>();
            button.name = "FRT_" + spriteName;
            button.atlas = m_atlas;
            button.relativePosition = new Vector2(0, 0);

            button.tooltip = toolTip + "\n\n" + string.Format(
                FineRoadTool.translation.GetTranslation("FRT_OPBTN_CYCLES_TOOLTIP"),
                OptionsKeymapping.modesCycleLeft.ToLocalizedString("KEYNAME"),
                OptionsKeymapping.modesCycleRight.ToLocalizedString("KEYNAME")
                );

            button.normalBgSprite = "OptionBase";
            button.hoveredBgSprite = "OptionBaseHovered";
            button.pressedBgSprite = "OptionBasePressed";
            button.disabledBgSprite = "OptionBaseDisabled";

            button.normalFgSprite = spriteName;
            button.hoveredFgSprite = spriteName + "Hovered";
            button.pressedFgSprite = spriteName + "Pressed";
            button.disabledFgSprite = spriteName + "Disabled";

            checkBox.eventCheckChanged += (c, s) =>
            {
                if (s)
                {
                    button.normalBgSprite = "OptionBaseFocused";
                    button.normalFgSprite = spriteName + "Focused";
                }
                else
                {
                    button.normalBgSprite = "OptionBase";
                    button.normalFgSprite = spriteName;
                }

                UpdateMode();
            };

            return checkBox;
        }

        private void UpdateMode()
        {
            if (m_normalModeButton.isChecked) FineRoadTooLoader.instance.mode = Mode.Normal;
            if (m_groundModeButton.isChecked) FineRoadTooLoader.instance.mode = Mode.Ground;
            if (m_elevatedModeButton.isChecked) FineRoadTooLoader.instance.mode = Mode.Elevated;
            if (m_bridgeModeButton.isChecked) FineRoadTooLoader.instance.mode = Mode.Bridge;
            if (m_tunnelModeButton.isChecked) FineRoadTooLoader.instance.mode = Mode.Tunnel;
        }

        private void LoadResources()
        {
            string[] spriteNames = new string[]
			{
				"NormalMode",
				"NormalModeDisabled",
				"NormalModeFocused",
				"NormalModeHovered",
				"NormalModePressed",
				"BridgeMode",
				"BridgeModeDisabled",
				"BridgeModeFocused",
				"BridgeModeHovered",
				"BridgeModePressed",
				"ElevatedMode",
				"ElevatedModeDisabled",
				"ElevatedModeFocused",
				"ElevatedModeHovered",
				"ElevatedModePressed",
				"GroundMode",
				"GroundModeDisabled",
				"GroundModeFocused",
				"GroundModeHovered",
				"GroundModePressed",
				"TunnelMode",
				"TunnelModeDisabled",
				"TunnelModeFocused",
				"TunnelModeHovered",
				"TunnelModePressed"
			};

            m_atlas = ResourceLoader.CreateTextureAtlas("FineRoadTool", spriteNames, "FineRoadTool.Icons.");

            UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures = new Texture2D[]
            {
                defaultAtlas["OptionBase"].texture,
                defaultAtlas["OptionBaseFocused"].texture,
                defaultAtlas["OptionBaseHovered"].texture,
                defaultAtlas["OptionBasePressed"].texture,
                defaultAtlas["OptionBaseDisabled"].texture
            };

            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);
        }
    }
}
