using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Button;
using MelonLoader;

namespace AutoRelog
{
    public static class BuildInfo
    {
        public const string Name = "AutoRelog";
        public const string Author = "Slaynash";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }
    
    public class AutoRelog : MelonMod
    {
        public override VRChat_OnUiManagerInit()
        {
            var authPages = Resources.FindObjectsOfTypeAll<VRCUiPageAuthentication>();
            VRCUiPageAuthentication loginPage = null;
            foreach (var authPage in authPages) // I don't trust LINQ + the il2cpp trash
            {
                if (authPage.gameObject.name == "LoginUserPass")
                {
                    loginPage = authPage;
                    break;
                }
            }

            if (loginPage == null) return;
            var loginButton = loginPage.transform.Find("ButtonDone (1)")?.GetComponent<Button>();
            if (loginButton == null) return;
            
            var bce = loginButton.onClick;
            loginButton.onClick = new Button.ButtonClickedEvent();
            loginButton.onClick.AddListener(new Action(() => {
                SecurePlayerPrefs.SetString("autorelog_login", GetTextFromUiInputField(loginPage.loginUserName), "vl9u1grTnvXA");
                SecurePlayerPrefs.SetString("autorelog_password", GetTextFromUiInputField(loginPage.loginPassword), "vl9u1grTnvXA");
                bce?.Invoke();
            }));

            var useprebiousTransform = UnityUiUtils.DuplicateButton(loginButton.transform, "Use Last\nCredentials", new Vector2(440, 0));
            useprebiousTransform.GetComponent<RectTransform>().sizeDelta *= 1.8f;
            var useprebiousButton = useprebiousTransform.GetComponent<Button>();
            useprebiousButton.onClick = new Button.ButtonClickedEvent();
            useprebiousButton.onClick.AddListener(new Action(() => {
                SetTextToUiInputField(loginPage.loginUserName, SecurePlayerPrefs.GetString("autorelog_login", "vl9u1grTnvXA"));
                SetTextToUiInputField(loginPage.loginPassword, SecurePlayerPrefs.GetString("autorelog_password", "vl9u1grTnvXA"));
            }));
            if (!SecurePlayerPrefs.HasKey("autorelog_login"))
            {
                useprebiousButton.interactable = false;
            }
        }

        private static string GetTextFromUiInputField(UiInputField field)
        {
            /*
            FieldInfo textField = typeof(UiInputField).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).First(f => f.FieldType == typeof(string) && f.Name != "placeholderInputText");
            return textField.GetValue(field) as string;
            */
            return field.prop_String_0;
        }

        private static void SetTextToUiInputField(UiInputField field, string text)
        {
            field.prop_String_0 = text;
            field.GetComponent<InputFieldValidator>()?.Method_Public_String_0(text);
        }
    }
}
