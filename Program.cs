using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class SortingBenchmark
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Розміри масивів для тестування
        int[] sizes = { 10, 1000, 10000, 1000000, 100000000 };

        // Методи сортування для порівняння
        string[] methods = { "Insertion Sort", "Bubble Sort", "Quick Sort", "Merge Sort", "Count Sort", "Radix Sort", "Busket Sort", "Timsort", "Binary Tree Sort" };

        // Порівняння випадкових масивів з великого інтервалу
        Console.WriteLine("Порівняння часу сортування (в мілісекундах) для випадкових масивів з великого інтервалу\n");
        CompareSortingTimes(sizes, methods, 1, 1000000);

        // Порівняння випадкових масивів з малого інтервалу
        Console.WriteLine("Порівняння часу сортування (в мілісекундах) для випадкових масивів з малого інтервалу\n");
        CompareSortingTimes(sizes, methods, 1, 10);

        //Порівняння масивів, відсортованих за спаданням, з великого інтервалу
        Console.WriteLine("Порівняння часу сортування (в мілісекундах) для відсортованих за спаданням масивів з великого інтервалу\n");
        CompareSortingTimes(sizes, methods, 1, 1000000, true);

        // Порівняння масивів, відсортованих за спаданням, з малого інтервалу
        Console.WriteLine("Порівняння часу сортування (в мілісекундах) для відсортованих за спаданням масивів з малого інтервалу\n");
        CompareSortingTimes(sizes, methods, 1, 10, true);
    }

    // Функція порівняння часу сортування для різних методів
    static void CompareSortingTimes(int[] sizes, string[] methods, float minValue, float maxValue, bool descending = false)
    {
        // Вивід заголовку таблиці
        Console.WriteLine("{0,-20} {1,10} {2,10} {3,10} {4,10} {5,10}", "Метод", sizes[0], sizes[1], sizes[2], sizes[3], sizes[4]);

        // Порівняння кожного методу сортування для кожного розміру масиву
        foreach (var method in methods)
        {
            Console.Write("{0,-20}", method);
            foreach (var size in sizes)
            {
                // Генерація випадкового масиву
                float[] array = GenerateRandomArray(size, minValue, maxValue);

                // Якщо необхідно, сортуємо масив за спаданням
                if (descending)
                {
                    Array.Sort(array);
                    Array.Reverse(array);
                }

                long elapsedMilliseconds = SortWithTimeout(method, array, TimeSpan.FromSeconds(10));

                // Вивід часу сортування
                Console.Write("{0,10}", elapsedMilliseconds >= 0 ? elapsedMilliseconds : "Timeout");
            }
            Console.WriteLine();
        }
    }

    // Функція для сортування з обмеженням за часом
    static long SortWithTimeout(string method, float[] array, TimeSpan timeout)
    {
        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(timeout);

            var task = Task.Run(() =>
            {
                var watch = Stopwatch.StartNew();
                Sort(method, array);
                watch.Stop();
                return watch.ElapsedMilliseconds;
            }, cancellationTokenSource.Token);

            if (task.Wait(timeout))
            {
                return task.Result;
            }
            else
            {
                cancellationTokenSource.Cancel();
                return -1; // Час вичерпано
            }
        }
        catch (AggregateException)
        {
            return -1; // Час вичерпано
        }
    }

    // Функція генерації випадкового масиву
    static float[] GenerateRandomArray(int size, float minValue, float maxValue)
    {
        var rand = new Random();
        float[] array = new float[size];
        for (int i = 0; i < size; i++)
        {
            array[i] = (float)(rand.NextDouble() * (maxValue - minValue) + minValue);
        }
        return array;
    }

    // Функція вибору методу сортування
    static void Sort(string method, float[] array)
    {
        switch (method)
        {
            case "Insertion Sort":
                InsertionSort(array);
                break;
            case "Bubble Sort":
                BubbleSort(array);
                break;
            case "Quick Sort":
                QuickSort(array);
                break;
            case "Merge Sort":
                MergeSort(array, 0, array.Length - 1);
                break;
            case "Count Sort":
                CountSort(array);
                break;
            case "Radix Sort":
                RadixSort(array);
                break;
            case "Busket Sort":
                BusketSort(array);
                break;
            case "Timsort":
                Timsort(array);
                break;
            case "Binary Tree Sort":
                BinaryTreeSort(array);
                break;
        }
    }

    // Метод сортування вставками
    static void InsertionSort(float[] array)
    {
        for (int i = 1; i < array.Length; i++)
        {
            float key = array[i];
            int j = i - 1;
            while (j >= 0 && array[j] > key)
            {
                array[j + 1] = array[j];
                j = j - 1;
            }
            array[j + 1] = key;
        }
    }

    // Метод сортування бульбашкою
    static void BubbleSort(float[] array)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
            for (int j = 0; j < n - 1 - i; j++)
                if (array[j] > array[j + 1])
                {
                    float temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;
                }
    }

    // Метод швидкого сортування
    static void QuickSort(float[] array)
    {
        int[] stack = new int[array.Length];
        int top = -1;

        stack[++top] = 0;
        stack[++top] = array.Length - 1;

        while (top >= 0)
        {
            int high = stack[top--];
            int low = stack[top--];

            int p = Partition(array, low, high);

            if (p - 1 > low)
            {
                stack[++top] = low;
                stack[++top] = p - 1;
            }

            if (p + 1 < high)
            {
                stack[++top] = p + 1;
                stack[++top] = high;
            }
        }
    }

    // Функція розбиття для швидкого сортування
    static int Partition(float[] array, int low, int high)
    {
        float pivot = array[high];
        int i = (low - 1);
        for (int j = low; j < high; j++)
        {
            if (array[j] < pivot)
            {
                i++;
                float temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }
        float temp1 = array[i + 1];
        array[i + 1] = array[high];
        array[high] = temp1;
        return i + 1;
    }

    // Метод сортування злиттям
    static void MergeSort(float[] array, int left, int right)
    {
        if (left < right)
        {
            int middle = (left + right) / 2;
            MergeSort(array, left, middle);
            MergeSort(array, middle + 1, right);
            Merge(array, left, middle, right);
        }
    }

    // Функція злиття для сортування злиттям
    static void Merge(float[] array, int left, int middle, int right)
    {
        int n1 = middle - left + 1;
        int n2 = right - middle;
        float[] leftArray = new float[n1];
        float[] rightArray = new float[n2];
        Array.Copy(array, left, leftArray, 0, n1);
        Array.Copy(array, middle + 1, rightArray, 0, n2);

        int i = 0, j = 0, k = left;
        while (i < n1 && j < n2)
        {
            if (leftArray[i] <= rightArray[j])
            {
                array[k] = leftArray[i];
                i++;
            }
            else
            {
                array[k] = rightArray[j];
                j++;
            }
            k++;
        }
        while (i < n1)
        {
            array[k] = leftArray[i];
            i++;
            k++;
        }
        while (j < n2)
        {
            array[k] = rightArray[j];
            j++;
            k++;
        }
    }

    // Метод підрахункового сортування
    static void CountSort(float[] array)
    {
        float max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
                max = array[i];
        }

        int[] count = new int[(int)max + 1];
        for (int i = 0; i < array.Length; i++)
        {
            count[(int)array[i]]++;
        }

        int index = 0;
        for (int i = 0; i < count.Length; i++)
        {
            while (count[i] > 0)
            {
                array[index] = i;
                index++;
                count[i]--;
            }
        }
    }

    // Метод сортування радіксом
    static void RadixSort(float[] array)
    {
        float max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
                max = array[i];
        }

        for (int exp = 1; max / exp > 0; exp *= 10)
        {
            CountSortByDigit(array, exp);
        }
    }

    // Функція підрахункового сортування для кожного розряду в радікс сортуванні
    static void CountSortByDigit(float[] array, int exp)
    {
        int n = array.Length;
        float[] output = new float[n];
        int[] count = new int[10];

        for (int i = 0; i < n; i++)
        {
            count[(int)((array[i] / exp) % 10)]++;
        }

        for (int i = 1; i < 10; i++)
        {
            count[i] += count[i - 1];
        }

        for (int i = n - 1; i >= 0; i--)
        {
            output[count[(int)((array[i] / exp) % 10)] - 1] = array[i];
            count[(int)((array[i] / exp) % 10)]--;
        }

        for (int i = 0; i < n; i++)
        {
            array[i] = output[i];
        }
    }

    // Метод Busket Sort
    static void BusketSort(float[] array)
    {
        if (array.Length == 0)
            return;

        float minValue = array[0];
        float maxValue = array[0];

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] < minValue)
                minValue = array[i];
            else if (array[i] > maxValue)
                maxValue = array[i];
        }

        int BusketCount = (int)(maxValue - minValue) / 9; // Зміна кількості кошиків
        List<float>[] Buskets = new List<float>[BusketCount];

        for (int i = 0; i < Buskets.Length; i++)
        {
            Buskets[i] = new List<float>();
        }

        for (int i = 0; i < array.Length; i++)
        {
            int BusketIndex = (int)((array[i] - minValue) / 10); // Зміна індексу кошика
            Buskets[BusketIndex].Add(array[i]);
        }

        int k = 0;
        for (int i = 0; i < Buskets.Length; i++)
        {
            foreach (float value in Buskets[i])
            {
                array[k++] = value;
            }
        }
    }

    // Метод Timsort
    static void Timsort(float[] array)
    {
        Array.Sort(array);
    }

    // Метод Binary Tree Sort    
    static void BinaryTreeSort(int[] array)
    {
        if (array.Length == 0)
            return;

        TreeNode root = new TreeNode(array[0]);

        for (int i = 1; i < array.Length; i++)
        {
            root.Insert(array[i]);
        }

        int index = 0;
        InOrderTraversal(root, array, ref index);
    }

    // Функція обхід дерева в порядку (In-Order Traversal)
    static void InOrderTraversal(TreeNode node, int[] array, ref int index)
    {
        if (node != null)
        {
            InOrderTraversal(node.Left, array, ref index);
            array[index++] = node.Value;
            InOrderTraversal(node.Right, array, ref index);
        }
    }

    // Клас вузла дерева
    class TreeNode
    {
        public int Value;
        public TreeNode Left;
        public TreeNode Right;

        public TreeNode(int value)
        {
            Value = value;
            Left = null;
            Right = null;
        }

        public void Insert(int value)
        {
            if (value <= Value)
            {
                if (Left == null)
                {
                    Left = new TreeNode(value);
                }
                else
                {
                    Left.Insert(value);
                }
            }
            else
            {
                if (Right == null)
                {
                    Right = new TreeNode(value);
                }
                else
                {
                    Right.Insert(value);
                }
            }
        }
    }
}
