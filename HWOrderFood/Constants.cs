using System;
namespace HWOrderFood
{
    public static class Constants
    {
#if DEBUG
        public static string HWOrderFood_BASE_URL = "http://206.189.61.227/foodDelivery/v1/";
#else
        public static string HWOrderFood_BASE_URL = "http://206.189.61.227/foodDelivery/v1/";
#endif

        #region -- Image Source --

        public static string CheckboxOnImageSource = "checkbox_on";

        public static string CheckboxOffImageSource = "checkbox_off";

        public static string MoreImageSource = "more";

        public static string LessImageSource = "less";

        #endregion

        #region -- Errors --

        public static string ErrorWithSendDataToServer = "Ошибка передачи данных на сервер";

        #endregion
    }
}
