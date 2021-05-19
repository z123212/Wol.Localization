using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Wol.Localization.Injections
{
    [HarmonyPatch]
    class CharacterSheetFixPatch
    {


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterSheet), "SetFocus", new Type[] { typeof(CharacterSheet.Focus) })]
        static bool SetFocusPatch(CharacterSheet __instance, CharacterSheet.Focus focus,ref CharacterSheet.Focus ___m_focus,ref MPlayer ___m_player, string ___m_strEnchFocused)
        {
            //Debug.LogError("CharacterSheet.SetFocus innnnnnn");
            var _this = __instance;
            var trav = Traverse.Create(__instance);
            ___m_focus = focus;
            //Traverse.Create(__instance).Field("m_focus").SetValue(focus);
            //_this.m_focus = focus;
            switch (focus)
            {
                case CharacterSheet.Focus.Portrait:
                    SkillListing.DeselectAll();

                    if (_this.textInfo != null)
                    {
                        var playStr = ___m_player.StrCharSheetStats();//trav.Field("m_player").Method("StrCharSheetStats").GetValue<string>();

                        _this.textInfo.text = playStr;//_this.m_player.StrCharSheetStats();
                    }
                    break;
                case CharacterSheet.Focus.Skill:
                    {
                        SkillListingLine selected = SkillListing.selected;
                        if (_this.textInfo != null)
                        {
                            if (selected != null)
                            {
                                MSkill skill = selected.skill;
                                StringBuilder stringBuilder = new StringBuilder();
                                stringBuilder.Append(skill.GetString("description", "", true));
                                bool @bool = skill.GetBool("nopreview", false, false);
                                SkillListing.SkillType type = selected.type;
                                if (type - SkillListing.SkillType.Stats > 2)
                                {
                                    if (type == SkillListing.SkillType.Perks)
                                    {
                                        string value = skill.StrEnchantments(selected.level);
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            stringBuilder.Append("\n\n");
                                            stringBuilder.Append("<b><color=blue>");
                                            stringBuilder.Append(value);
                                            stringBuilder.Append("</color></b>");
                                        }
                                    }
                                }
                                else if (selected.level < skill.levels.Count)
                                {
                                    if (!MPlayer.instance.autoLevel)
                                    {
                                        stringBuilder.Append("\n\n");
                                        stringBuilder.Append(skill.XpToUpgrade(selected.level + 1));
                                        stringBuilder.Append(string.Format(" {0} ", Localization.GetLocalizationStringAndWrite("XP to increase.")));
                                        stringBuilder.Append('\n');
                                        stringBuilder.Append(string.Format("{0} ", Localization.GetLocalizationStringAndWrite("You have")));
                                        stringBuilder.Append(MPlayer.instance.xp);
                                        stringBuilder.Append(" XP.");
                                    }
                                    if (!@bool)
                                    {
                                        string value2 = skill.StrEnchantments(selected.level);
                                        if (!string.IsNullOrEmpty(value2))
                                        {
                                            stringBuilder.Append(string.Format("\n\n{0}\n", Localization.GetLocalizationStringAndWrite("At this level:")));
                                            stringBuilder.Append("<b><color=blue>");
                                            stringBuilder.Append(value2);
                                            stringBuilder.Append("</color></b>");
                                        }
                                    }
                                    string text = skill.StrBlueText(selected.level);
                                    if (text != null)
                                    {
                                        stringBuilder.Append('\n');
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(text);
                                        stringBuilder.Append("</color></b>");
                                    }
                                    if (!@bool)
                                    {
                                        string value3 = skill.StrEnchantments(selected.level + 1);
                                        if (!string.IsNullOrEmpty(value3))
                                        {
                                            stringBuilder.Append(string.Format("\n\n{0}\n", Localization.GetLocalizationStringAndWrite("At next level:")));
                                            stringBuilder.Append("<b><color=blue>");
                                            stringBuilder.Append(value3);
                                            stringBuilder.Append("</color></b>");
                                        }
                                    }
                                    text = skill.StrBlueText(selected.level + 1);
                                    if (text != null)
                                    {
                                        stringBuilder.Append('\n');
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(text);
                                        stringBuilder.Append("</color></b>");
                                    }
                                }
                                else
                                {
                                    if (!@bool)
                                    {
                                        string value4 = skill.StrEnchantments(selected.level);
                                        if (!string.IsNullOrEmpty(value4))
                                        {
                                            stringBuilder.Append("\n\n");
                                            stringBuilder.Append("<b><color=blue>");
                                            stringBuilder.Append(value4);
                                            stringBuilder.Append("</color></b>");
                                        }
                                    }
                                    string text2 = skill.StrBlueText(selected.level);
                                    if (text2 != null)
                                    {
                                        stringBuilder.Append('\n');
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(text2);
                                        stringBuilder.Append("</color></b>");
                                    }
                                }
                                //trav.Field("textInfo").Property("text").SetValue(MCommand.MadlibString(stringBuilder.ToString()));
                                _this.textInfo.text = MCommand.MadlibString(stringBuilder.ToString());
                            }
                            else
                            {

                                _this.textInfo.text = "";
                            }
                        }
                        break;
                    }
                case CharacterSheet.Focus.Enchantment:
                    SkillListing.DeselectAll();
                    if (_this.textInfo != null)
                    {
                        //trav.Field("textInfo").Property("text").SetValue(trav.Field("m_strEnchFocused").GetValue<string>());
                        _this.textInfo.text = ___m_strEnchFocused;//trav.Field("m_strEnchFocused").GetValue<string>();

                    }
                    break;
                default:
                    if (_this.textInfo != null)
                    {
                        //trav.Field("textInfo").Property("text").SetValue("");
                        _this.textInfo.text = "";
                    }
                    break;
            }
            ScrollRect componentInParent = _this.textInfo.GetComponentInParent<ScrollRect>();
            if (componentInParent != null)
            {
                componentInParent.normalizedPosition = Vector2.up;
            }
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterSheetSwitch), "SetFocus", new Type[] { typeof(CharacterSheetSwitch.Focus) })]
        static bool SwitchSetFocusPatch(CharacterSheetSwitch __instance, CharacterSheetSwitch.Focus focus, ref CharacterSheetSwitch.Focus ___m_focus, ref MPlayer ___m_player, string ___m_strEnchFocused)
        {
            var _this = __instance;
            var trav = Traverse.Create(__instance);
            ___m_focus = focus;//Traverse.Create(__instance).Field("m_focus").SetValue(focus);

            //this.m_focus = focus;
            bool active = false;
            bool interactable = false;
            switch (focus)
            {
                case CharacterSheetSwitch.Focus.Portrait:
                    if (_this.textInfo != null)
                    {
                        _this.textInfo.text = ___m_player.StrCharSheetStats();// trav.Field("m_player").Method("StrCharSheetStats").GetValue<string>();
                        //_this.m_player.StrCharSheetStats();
                    }
                    break;
                case CharacterSheetSwitch.Focus.Skill:
                    if (_this.textInfo != null)
                    {
                        SkillListingLine selected = SkillListing.selected;
                        if (selected != null)
                        {
                            MSkill skill = selected.skill;
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.Append(skill.GetString("description", "", true));
                            bool @bool = skill.GetBool("nopreview", false, false);
                            SkillListing.SkillType type = selected.type;
                            if (type - SkillListing.SkillType.Stats > 2)
                            {
                                if (type == SkillListing.SkillType.Perks)
                                {
                                    string value = skill.StrEnchantments(selected.level);
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        stringBuilder.Append("\n\n");
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(value);
                                        stringBuilder.Append("</color></b>");
                                    }
                                }
                            }
                            else if (selected.level < skill.levels.Count)
                            {
                                if (!MPlayer.instance.autoLevel)
                                {
                                    int num = skill.XpToUpgrade(selected.level + 1);
                                    int xp = MPlayer.instance.xp;
                                    stringBuilder.Append("\n\n");
                                    stringBuilder.Append(num);
                                    stringBuilder.Append(string.Format(" {0}", Localization.GetLocalizationStringAndWrite("XP to increase.")));
                                    stringBuilder.Append('\n');
                                    stringBuilder.Append(string.Format("{0} ", Localization.GetLocalizationStringAndWrite("You have")));
                                    stringBuilder.Append(xp);
                                    stringBuilder.Append(" XP.");
                                    active = true;
                                    interactable = (xp >= num);
                                    if (_this.upgradeLabel != null)
                                    {
                                        string localizationStringAndWrite = Localization.GetLocalizationStringAndWrite(_this.upgradeLabelFormat);
                                        _this.upgradeLabel.text = string.Format(localizationStringAndWrite, num);
                                    }
                                }
                                if (!@bool)
                                {
                                    string value2 = skill.StrEnchantments(selected.level);
                                    if (!string.IsNullOrEmpty(value2))
                                    {
                                        stringBuilder.Append(string.Format("\n\n{0}\n", Localization.GetLocalizationStringAndWrite("At this level:")));
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(value2);
                                        stringBuilder.Append("</color></b>");
                                    }
                                }
                                string text = skill.StrBlueText(selected.level);
                                if (text != null)
                                {
                                    stringBuilder.Append('\n');
                                    stringBuilder.Append("<b><color=blue>");
                                    stringBuilder.Append(text);
                                    stringBuilder.Append("</color></b>");
                                }
                                if (!@bool)
                                {
                                    string value3 = skill.StrEnchantments(selected.level + 1);
                                    if (!string.IsNullOrEmpty(value3))
                                    {
                                        stringBuilder.Append(string.Format("\n\n{0}\n", Localization.GetLocalizationStringAndWrite("At next level:")));
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(value3);
                                        stringBuilder.Append("</color></b>");
                                    }
                                }
                                text = skill.StrBlueText(selected.level + 1);
                                if (text != null)
                                {
                                    stringBuilder.Append('\n');
                                    stringBuilder.Append("<b><color=blue>");
                                    stringBuilder.Append(text);
                                    stringBuilder.Append("</color></b>");
                                }
                            }
                            else
                            {
                                if (!@bool)
                                {
                                    string value4 = skill.StrEnchantments(selected.level);
                                    if (!string.IsNullOrEmpty(value4))
                                    {
                                        stringBuilder.Append("\n\n");
                                        stringBuilder.Append("<b><color=blue>");
                                        stringBuilder.Append(value4);
                                        stringBuilder.Append("</color></b>");
                                    }
                                }
                                string text2 = skill.StrBlueText(selected.level);
                                if (text2 != null)
                                {
                                    stringBuilder.Append('\n');
                                    stringBuilder.Append("<b><color=blue>");
                                    stringBuilder.Append(text2);
                                    stringBuilder.Append("</color></b>");
                                }
                            }
                            _this.textInfo.text = MCommand.MadlibString(stringBuilder.ToString());
                        }
                        else
                        {
                            _this.textInfo.text = "";
                        }
                    }
                    break;
                case CharacterSheetSwitch.Focus.Enchantment:
                    SkillListing.DeselectAll();
                    if (_this.textInfo != null)
                    {
                        _this.textInfo.text = ___m_strEnchFocused;//trav.Field("m_strEnchFocused").GetValue<string>();
                        //_this.m_strEnchFocused;
                    }
                    break;
                case CharacterSheetSwitch.Focus.Effect:
                    if (_this.textInfo != null)
                    {
                        SkillListingLine selected2 = SkillListing.selected;
                        if (selected2 == null || selected2.effect == null)
                        {
                            _this.textInfo.text = null;
                        }
                        else
                        {
                            MEffect effect = selected2.effect;
                            _this.textInfo.text = effect.StrFullDescription(selected2.level);
                        }
                    }
                    break;
                default:
                    if (_this.textInfo != null)
                    {
                        _this.textInfo.text = "";
                    }
                    break;
            }
            ScrollRect componentInParent = _this.textInfo.GetComponentInParent<ScrollRect>();
            if (componentInParent != null)
            {
                componentInParent.normalizedPosition = Vector2.up;
            }
            if (_this.upgradeGroup != null)
            {
                _this.upgradeGroup.SetActive(active);
            }
            if (_this.upgradeButton != null)
            {
                _this.upgradeButton.interactable = interactable;
            }
            return false;
        }


    }

     
    [HarmonyPatch(typeof(CharacterSheetSwitch),MethodType.Constructor)]
    class CharacterSheetSwitchPatch
    {
        static void Postfix(CharacterSheetSwitch __instance)
        {
            __instance.upgradeLabelFormat = Localization.GetLocalizationStringAndWrite(__instance.upgradeLabelFormat);
        }

       
    }
}
