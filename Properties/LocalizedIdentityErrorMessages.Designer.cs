﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace stranitza.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class LocalizedIdentityErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal LocalizedIdentityErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("stranitza.Properties.LocalizedIdentityErrorMessages", typeof(LocalizedIdentityErrorMessages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Проблем при запис на данни: данните междувременно са били променени..
        /// </summary>
        public static string ConcurrencyFailure {
            get {
                return ResourceManager.GetString("ConcurrencyFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Възникнала е неразпозната грешка, моля, пробвайте отново..
        /// </summary>
        public static string DefaultError {
            get {
                return ResourceManager.GetString("DefaultError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email &apos;{0}&apos; е вече зает от друг потребител..
        /// </summary>
        public static string DuplicateEmail {
            get {
                return ResourceManager.GetString("DuplicateEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Роля &apos;{0}&apos; вече съществува..
        /// </summary>
        public static string DuplicateRoleName {
            get {
                return ResourceManager.GetString("DuplicateRoleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Псевдоним &apos;{0}&apos; е вече зает от друг потребител..
        /// </summary>
        public static string DuplicateUserName {
            get {
                return ResourceManager.GetString("DuplicateUserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email &apos;{0}&apos; адресът е невалиден..
        /// </summary>
        public static string InvalidEmail {
            get {
                return ResourceManager.GetString("InvalidEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type {0} must derive from {1}&lt;{2}&gt;..
        /// </summary>
        public static string InvalidManagerType {
            get {
                return ResourceManager.GetString("InvalidManagerType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The provided PasswordHasherCompatibilityMode is invalid..
        /// </summary>
        public static string InvalidPasswordHasherCompatibilityMode {
            get {
                return ResourceManager.GetString("InvalidPasswordHasherCompatibilityMode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The iteration count must be a positive integer..
        /// </summary>
        public static string InvalidPasswordHasherIterationCount {
            get {
                return ResourceManager.GetString("InvalidPasswordHasherIterationCount", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Името на ролята (&apos;{0}&apos;) не разрешено..
        /// </summary>
        public static string InvalidRoleName {
            get {
                return ResourceManager.GetString("InvalidRoleName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid token..
        /// </summary>
        public static string InvalidToken {
            get {
                return ResourceManager.GetString("InvalidToken", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Псевдоним &apos;{0}&apos; е невалидно, може да съдържа единствено букви и цифри..
        /// </summary>
        public static string InvalidUserName {
            get {
                return ResourceManager.GetString("InvalidUserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Вече съществува потребител, който използва този външен потребител за вход..
        /// </summary>
        public static string LoginAlreadyAssociated {
            get {
                return ResourceManager.GetString("LoginAlreadyAssociated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AddIdentity must be called on the service collection..
        /// </summary>
        public static string MustCallAddIdentity {
            get {
                return ResourceManager.GetString("MustCallAddIdentity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No IPersonalDataProtector service was registered, this is required when ProtectPersonalData = true..
        /// </summary>
        public static string NoPersonalDataProtector {
            get {
                return ResourceManager.GetString("NoPersonalDataProtector", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No RoleType was specified, try AddRoles&lt;TRole&gt;()..
        /// </summary>
        public static string NoRoleType {
            get {
                return ResourceManager.GetString("NoRoleType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No IUserTwoFactorTokenProvider&lt;{0}&gt; named &apos;{1}&apos; is registered..
        /// </summary>
        public static string NoTokenProvider {
            get {
                return ResourceManager.GetString("NoTokenProvider", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Печатът за защита на потребителя не съществува..
        /// </summary>
        public static string NullSecurityStamp {
            get {
                return ResourceManager.GetString("NullSecurityStamp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Невалидна парола..
        /// </summary>
        public static string PasswordMismatch {
            get {
                return ResourceManager.GetString("PasswordMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Паролата трябва да съдържа поне една цифра..
        /// </summary>
        public static string PasswordRequiresDigit {
            get {
                return ResourceManager.GetString("PasswordRequiresDigit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Паролата трябва да съдържа поне една малка буква..
        /// </summary>
        public static string PasswordRequiresLower {
            get {
                return ResourceManager.GetString("PasswordRequiresLower", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Паролата трябва да съдържа поне един специален символ..
        /// </summary>
        public static string PasswordRequiresNonAlphanumeric {
            get {
                return ResourceManager.GetString("PasswordRequiresNonAlphanumeric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Паролата трябва да съдържа поне {0} различни символа..
        /// </summary>
        public static string PasswordRequiresUniqueChars {
            get {
                return ResourceManager.GetString("PasswordRequiresUniqueChars", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Паролата трябва да съдържа поне една главна буква..
        /// </summary>
        public static string PasswordRequiresUpper {
            get {
                return ResourceManager.GetString("PasswordRequiresUpper", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Паролата трябва да е дълга поне {0} символа..
        /// </summary>
        public static string PasswordTooShort {
            get {
                return ResourceManager.GetString("PasswordTooShort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Възстановяването чрез кода за възстановяване не бе успешно..
        /// </summary>
        public static string RecoveryCodeRedemptionFailed {
            get {
                return ResourceManager.GetString("RecoveryCodeRedemptionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Роля {0} не съществува..
        /// </summary>
        public static string RoleNotFound {
            get {
                return ResourceManager.GetString("RoleNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IProtectedUserStore&lt;TUser&gt; which is required when ProtectPersonalData = true..
        /// </summary>
        public static string StoreNotIProtectedUserStore {
            get {
                return ResourceManager.GetString("StoreNotIProtectedUserStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IQueryableRoleStore&lt;TRole&gt;..
        /// </summary>
        public static string StoreNotIQueryableRoleStore {
            get {
                return ResourceManager.GetString("StoreNotIQueryableRoleStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IQueryableUserStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIQueryableUserStore {
            get {
                return ResourceManager.GetString("StoreNotIQueryableUserStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IRoleClaimStore&lt;TRole&gt;..
        /// </summary>
        public static string StoreNotIRoleClaimStore {
            get {
                return ResourceManager.GetString("StoreNotIRoleClaimStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserAuthenticationTokenStore&lt;User&gt;..
        /// </summary>
        public static string StoreNotIUserAuthenticationTokenStore {
            get {
                return ResourceManager.GetString("StoreNotIUserAuthenticationTokenStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserAuthenticatorKeyStore&lt;User&gt;..
        /// </summary>
        public static string StoreNotIUserAuthenticatorKeyStore {
            get {
                return ResourceManager.GetString("StoreNotIUserAuthenticatorKeyStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserClaimStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserClaimStore {
            get {
                return ResourceManager.GetString("StoreNotIUserClaimStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserConfirmationStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserConfirmationStore {
            get {
                return ResourceManager.GetString("StoreNotIUserConfirmationStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserEmailStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserEmailStore {
            get {
                return ResourceManager.GetString("StoreNotIUserEmailStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserLockoutStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserLockoutStore {
            get {
                return ResourceManager.GetString("StoreNotIUserLockoutStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserLoginStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserLoginStore {
            get {
                return ResourceManager.GetString("StoreNotIUserLoginStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserPasswordStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserPasswordStore {
            get {
                return ResourceManager.GetString("StoreNotIUserPasswordStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserPhoneNumberStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserPhoneNumberStore {
            get {
                return ResourceManager.GetString("StoreNotIUserPhoneNumberStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserRoleStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserRoleStore {
            get {
                return ResourceManager.GetString("StoreNotIUserRoleStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserSecurityStampStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserSecurityStampStore {
            get {
                return ResourceManager.GetString("StoreNotIUserSecurityStampStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserTwoFactorRecoveryCodeStore&lt;User&gt;..
        /// </summary>
        public static string StoreNotIUserTwoFactorRecoveryCodeStore {
            get {
                return ResourceManager.GetString("StoreNotIUserTwoFactorRecoveryCodeStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Store does not implement IUserTwoFactorStore&lt;TUser&gt;..
        /// </summary>
        public static string StoreNotIUserTwoFactorStore {
            get {
                return ResourceManager.GetString("StoreNotIUserTwoFactorStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Потребителят вече има зададена парола..
        /// </summary>
        public static string UserAlreadyHasPassword {
            get {
                return ResourceManager.GetString("UserAlreadyHasPassword", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Потребителят вече има роля &apos;{0}&apos;..
        /// </summary>
        public static string UserAlreadyInRole {
            get {
                return ResourceManager.GetString("UserAlreadyInRole", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Потребителят е заключен..
        /// </summary>
        public static string UserLockedOut {
            get {
                return ResourceManager.GetString("UserLockedOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Заключването за потребителя не е активирано..
        /// </summary>
        public static string UserLockoutNotEnabled {
            get {
                return ResourceManager.GetString("UserLockoutNotEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Потребител {0} не съществува..
        /// </summary>
        public static string UserNameNotFound {
            get {
                return ResourceManager.GetString("UserNameNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Потребителят няма роля &apos;{0}&apos;..
        /// </summary>
        public static string UserNotInRole {
            get {
                return ResourceManager.GetString("UserNotInRole", resourceCulture);
            }
        }
    }
}