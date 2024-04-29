namespace Currency.Exchange.API.Helper
{
    public static class Extensions
    {
        /// <summary>
        /// This will check if the object has property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type obj, string propertyName)
        {
            return obj.GetProperty(propertyName.ToUpper()) != null;
        }

        /// <summary>
        /// This will round the decimal up to 6 places.
        /// </summary>
        /// <param name="returnAmt"></param>
        /// <returns></returns>
        public static decimal GetDecimalPoint(this decimal returnAmt)
        {
            return Math.Round(returnAmt, 6);
        }
    }
}
