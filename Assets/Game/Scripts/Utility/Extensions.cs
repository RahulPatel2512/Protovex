using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Utility
{
    public static class Extensions
    {
        #region NO-LINQ Arrays, Lists and Collectables Extensions

        public static T[] ConcatArrays<T>(this T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            Array.Copy(array1, 0, result, 0, array1.Length);
            Array.Copy(array2, 0, result, array1.Length, array2.Length);

            return result;
        }

        public static T ElementAtOrDefault<T>(this T[] array, int index)
        {
            if (array == null || index < 0 || index >= array.Length)
            {
                return default; // Return default value of type T
            }

            return array[index];
        }

        public static T ElementAtOrDefault<T>(this List<T> list, int index)
        {
            if (list == null || index < 0 || index >= list.Count)
            {
                return default; // Return default value of type T
            }

            return list[index];
        }

        public static T FirstOrDefault<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (T item in list)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return default; // Return default value if no match is found
        }

        public static bool Any<T>(this T[] array, Func<T, bool> predicate)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (T item in array)
            {
                if (predicate(item)) // If any item matches the predicate
                {
                    return true; // Return true as soon as we find a matching element
                }
            }

            return false; // Return false if no items match the predicate
        }

        public static bool Any<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (T item in list)
            {
                if (predicate(item)) // If any item matches the predicate
                {
                    return true; // Return true as soon as we find a matching element
                }
            }

            return false; // Return false if no items match the predicate
        }

        public static bool All<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (T item in list)
            {
                if (!predicate(item)) // If any item doesn't match the predicate
                {
                    return false; // Return false immediately
                }
            }

            return true; // Return true if all items match the predicate
        }

        public static int Count<T>(this T[] array, Func<T, bool> predicate)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            int count = 0;

            foreach (T item in array)
            {
                if (predicate(item)) // Apply the predicate to each element
                {
                    count++; // Increment the count if the predicate returns true
                }
            }

            return count; // Return the final count
        }

        public static int Sum<T>(this List<T> list, Func<T, int> selector)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            int sum = 0;

            foreach (T item in list)
            {
                sum += selector(item); // Apply the selector function and add the result to the sum
            }

            return sum;
        }

        public static float Sum<T>(this List<T> list, Func<T, float> selector)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            float sum = 0;

            foreach (T item in list)
            {
                sum += selector(item); // Apply the selector function and add the result to the sum
            }

            return sum;
        }

        public static T GetWeightedItemFromProbabilityMap<T>(this List<T> items, Func<T, float> probabilitySelector)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("The item list cannot be null or empty.");

            float totalWeight = items.Sum(item => probabilitySelector(item));

            if (totalWeight <= 0)
                throw new ArgumentException("The total probability weight must be greater than zero.");

            float randomValue = UnityEngine.Random.Range(0, totalWeight);

            // Iterate through the list to find the selected item
            float cumulativeWeight = 0f;

            foreach (var item in items)
            {
                cumulativeWeight += probabilitySelector(item);
                if (randomValue <= cumulativeWeight)
                    return item;
            }

            return default;
        }

        public static TResult[] Select<T, TResult>(this T[] array, Func<T, TResult> selector)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            TResult[] result = new TResult[array.Length]; // Create a new array with the same length as the input array

            for (int i = 0; i < array.Length; i++)
            {
                result[i] = selector(array[i]); // Apply the selector to each element and store the result
            }

            return result;
        }

        public static List<TResult> Select<T, TResult>(this List<T> list, Func<T, TResult> selector)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            List<TResult> result = new List<TResult>(list.Count); // Create a list with the same capacity as the input

            foreach (T item in list)
            {
                result.Add(selector(item)); // Apply the transformation function to each element
            }

            return result;
        }

        public static List<T> Take<T>(this List<T> list, int count)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (count <= 0)
            {
                return new List<T>(); // Return an empty list if count is zero or negative
            }

            int takeCount = Math.Min(count, list.Count); // Ensure we don't take more than the list contains
            List<T> result = new(takeCount);

            for (int i = 0; i < takeCount; i++)
            {
                result.Add(list[i]);
            }

            return result;
        }

        public static List<T> Where<T>(this List<T> list, Func<T, bool> predicate)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            List<T> result = new List<T>();

            foreach (T item in list)
            {
                if (predicate(item)) // Apply the predicate to each element
                {
                    result.Add(item); // Add the element to the result list if it satisfies the predicate
                }
            }

            return result; // Return the filtered list
        }

        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(
            this List<T> list,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

            foreach (var item in list)
            {
                TKey key = keySelector(item);
                TValue value = valueSelector(item);

                if (!dictionary.ContainsKey(key))
                {
                    dictionary.Add(key, value);
                }
                else
                {
                    throw new ArgumentException("An element with the same key already exists.");
                }
            }

            return dictionary;
        }

        #endregion

        public static (bool, float) AreAnglesApproximatelyMultipleOf360(this float angleA, float angleB,
            float tolerance = 0.1f)
        {
            float difference = Mathf.Abs(angleA - angleB) % 360f;
            while (difference > 360f)
            {
                difference -= 360f;
            }

            return (difference <= tolerance, difference);
        }

        public static System.TimeSpan GetTimeSpan(this System.DateTime startTime, int hours, int minutes = 0,
            int seconds = 0)
        {
            return new System.TimeSpan(hours, minutes, seconds) - (System.DateTime.UtcNow - startTime);
        }

        public static void SnapTo(this ScrollRect scrollRect, RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.content.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }

        public static bool Roll(float chance) => Random.Range(0f, 1f) < chance;

        static string[] NumberSuffixes = new[]
        {
            "", "K", "M", "B", "T", "Q", "N", "S", "P", "O",
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
            "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"
        };

        public static string ToFormattedNumber(this float number, string formatForFractionalNumber = "0.0#")
        {
            if (number < 1000)
            {
                return Mathf.FloorToInt(number).ToString();
            }

            var tempNumber = number;
            var suffixIndex = 0;

            while (tempNumber >= 1000 && suffixIndex < NumberSuffixes.Length - 1)
            {
                tempNumber /= 1000;
                suffixIndex++;
            }

            var format = tempNumber % 1 == 0 ? "0" : formatForFractionalNumber;
            return tempNumber.ToString(format) + NumberSuffixes[suffixIndex];
        }

        public static float ParseFormattedNumber(this string input)
        {
            // Extract the numeric part and the suffix
            string numberPart = input.Substring(0, input.Length - 1);
            char suffix = input[input.Length - 1];

            // Convert the numeric part to float
            if (float.TryParse(numberPart, out float number))
            {
                // Find the index of the suffix
                int suffixIndex = System.Array.IndexOf(NumberSuffixes, suffix.ToString());

                // If the suffix is found and index is valid
                if (suffixIndex >= 0)
                {
                    // Calculate the multiplier as 1000 raised to the power of the suffix index
                    float multiplier = (float)System.Math.Pow(1000, suffixIndex);
                    return number * multiplier;
                }
            }

            // If parsing fails or no suffix is found, return 0 or handle as necessary
            return 0;
        }

        public static string ToCleanString(this string str)
        {
            return str.Replace(" ", "").ToLower();
        }
    
        public static T GetOrAddComponent<T>(this Component obj) where T : Component
        {
            return GetOrAddComponent<T>(obj.gameObject);
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (!obj.TryGetComponent<T>(out T comp))
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }

        public static T GetRandom<T>(this List<T> list)
        {
            return list.ElementAtOrDefault(Random.Range(0, list.Count));
        }

        public static List<T> GetRandom<T>(this List<T> list, int count = 1)
        {
            var result = new List<T>();
            if (list.Count == 0) return result;
            return list.Shuffle().SafeTake(count);
        }

        public static List<T> SafeTake<T>(this List<T> list, int count)
        {
            return list.Take(Mathf.Min(list.Count, count));
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                result.Add(item);
            }

            result.ShuffleInPlace<T>();
            return result;
        }

        public static IList<T> ShuffleInPlace<T>(this IList<T> list, int start = 0, int end = -1)
        {
            if (end == -1) end = list.Count - 1;

            for (int i = end; i > start; i--)
            {
                var j = Random.Range(0, i + 1);
                var value = list[j];
                list[j] = list[i];
                list[i] = value;
            }

            return list;
        }

        public static float Round(this float value, int decimalPlaces = 0)
        {
            var tens = Mathf.Pow(10, decimalPlaces);
            return Mathf.Round(value * tens) / tens;
        }

        public static string SetColor(this string str, Color color)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), str);
        }

        public static string ToSpriteString(this string str)
        {
            return $"<sprite name={str}>";
        }

        public static List<UI> DrawList<UI>(this Transform content, int count) where UI : Component
        {
            var existingUI = content.GetComponentsInChildren<UI>(true);
            var prefab = existingUI.ElementAtOrDefault(0);

            foreach (var ui in existingUI)
            {
                ui.Despawn();
            }

            var result = new List<UI>();

            for (int i = 0; i < count; i++)
            {
                var ui = prefab.Spawn(content);
                result.Add(ui);
            }

            return result;
        }

        //public static List<UI> DrawList<UI, Data>(this RectTransform content, IEnumerable<Data> list, System.Action<UI, Data> onDraw,
        //    System.Func<Data, int, UI> getPrefab = null) where UI : Component
        //{
        //    var existingUI = content.GetComponentsInChildren<UI>(true);
        //    var prefab = existingUI.FirstOrDefault();

        //    foreach (var ui in existingUI)
        //    {
        //        ui.Despawn();
        //    }

        //    var result = new List<UI>();

        //    for (int i = 0; i < list.Count(); i++)
        //    {
        //        var data = list.ElementAt(i);

        //        if (getPrefab != null)
        //        {
        //            prefab = getPrefab.Invoke(data, i);
        //        }

        //        var ui = prefab.Spawn(content);
        //        onDraw?.Invoke(ui, list.ElementAt(i));
        //        result.Add(ui);
        //    }

        //    return result;
        //}
    }
}