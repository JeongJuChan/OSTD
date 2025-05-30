using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

/** Based on BigInteger.cs by ScottGarland from http://biginteger.codeplex.com/ */
namespace Keiwando.BigInteger
{
	using DType = System.UInt32; // This could be UInt32, UInt16 or Byte; not UInt64.

	#region DigitsArray
	internal class DigitsArray
	{
		internal DigitsArray(int size)
		{
			Allocate(size, 0);
		}

		internal DigitsArray(int size, int used)
		{
			Allocate(size, used);
		}

		internal DigitsArray(DType[] copyFrom)
		{
			Allocate(copyFrom.Length);
			CopyFrom(copyFrom, 0, 0, copyFrom.Length);
			ResetDataUsed();
		}

		internal DigitsArray(DigitsArray copyFrom)
		{
			Allocate(copyFrom.Count, copyFrom.DataUsed);
			Array.Copy(copyFrom.m_data, 0, m_data, 0, copyFrom.Count);
		}

		private DType[] m_data;

		internal static readonly DType AllBits;     // = ~((DType)0);
		internal static readonly DType HiBitSet;    // = 0x80000000;
		internal static int DataSizeOf
		{
			get { return sizeof(DType); }
		}

		internal static int DataSizeBits
		{
			get { return sizeof(DType) * 8; }
		}

		static DigitsArray()
		{
			unchecked
			{
				AllBits = (DType)~((DType)0);
				HiBitSet = (DType)(((DType)1) << (DataSizeBits) - 1);
			}
		}

		public void Allocate(int size)
		{
			Allocate(size, 0);
		}

		public void Allocate(int size, int used)
		{
			m_data = new DType[size + 1];
			m_dataUsed = used;
		}

		internal void CopyFrom(DType[] source, int sourceOffset, int offset, int length)
		{
			Array.Copy(source, sourceOffset, m_data, 0, length);
		}

		internal void CopyTo(DType[] array, int offset, int length)
		{
			Array.Copy(m_data, 0, array, offset, length);
		}

		internal DType this[int index]
		{
			get
			{
				if (index < m_dataUsed) return m_data[index];
				return (IsNegative ? (DType)AllBits : (DType)0);
			}
			set { m_data[index] = value; }
		}

		private int m_dataUsed;
		internal int DataUsed
		{
			get { return m_dataUsed; }
			set { m_dataUsed = value; }
		}

		internal int Count
		{
			get { return m_data.Length; }
		}

		internal bool IsZero
		{
			get { return m_dataUsed == 0 || (m_dataUsed == 1 && m_data[0] == 0); }
		}

		internal bool IsNegative
		{
			get { return (m_data[m_data.Length - 1] & HiBitSet) == HiBitSet; }
		}

		internal string GetDataAsString()
		{
			string result = "";
			foreach (DType data in m_data)
			{
				result += data + " ";
			}
			return result;
		}

		internal void ResetDataUsed()
		{
			m_dataUsed = m_data.Length;
			if (IsNegative)
			{
				while (m_dataUsed > 1 && m_data[m_dataUsed - 1] == AllBits)
				{
					--m_dataUsed;
				}
				m_dataUsed++;
			}
			else
			{
				while (m_dataUsed > 1 && m_data[m_dataUsed - 1] == 0)
				{
					--m_dataUsed;
				}
				if (m_dataUsed == 0)
				{
					m_dataUsed = 1;
				}
			}
		}

		internal int ShiftRight(int shiftCount)
		{
			return ShiftRight(m_data, shiftCount);
		}

		internal static int ShiftRight(DType[] buffer, int shiftCount)
		{
			int shiftAmount = DigitsArray.DataSizeBits;
			int invShift = 0;
			int bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
			{
				bufLen--;
			}

			for (int count = shiftCount; count > 0; count -= shiftAmount)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
					invShift = DigitsArray.DataSizeBits - shiftAmount;
				}

				ulong carry = 0;
				for (int i = bufLen - 1; i >= 0; i--)
				{
					ulong val = ((ulong)buffer[i]) >> shiftAmount;
					val |= carry;

					carry = ((ulong)buffer[i]) << invShift;
					buffer[i] = (DType)(val);
				}
			}

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
			{
				bufLen--;
			}

			return bufLen;
		}

		internal int ShiftLeft(int shiftCount)
		{
			return ShiftLeft(m_data, shiftCount);
		}

		internal static int ShiftLeft(DType[] buffer, int shiftCount)
		{
			int shiftAmount = DigitsArray.DataSizeBits;
			int bufLen = buffer.Length;

			while (bufLen > 1 && buffer[bufLen - 1] == 0)
			{
				bufLen--;
			}

			for (int count = shiftCount; count > 0; count -= shiftAmount)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
				}

				ulong carry = 0;
				for (int i = 0; i < bufLen; i++)
				{
					ulong val = ((ulong)buffer[i]) << shiftAmount;
					val |= carry;

					buffer[i] = (DType)(val & DigitsArray.AllBits);
					carry = (val >> DigitsArray.DataSizeBits);
				}

				if (carry != 0)
				{
					if (bufLen + 1 <= buffer.Length)
					{
						buffer[bufLen] = (DType)carry;
						bufLen++;
						carry = 0;
					}
					else
					{
						throw new OverflowException();
					}
				}
			}
			return bufLen;
		}

		internal int ShiftLeftWithoutOverflow(int shiftCount)
		{
			List<DType> temporary = new List<DType>(m_data);
			int shiftAmount = DigitsArray.DataSizeBits;

			for (int count = shiftCount; count > 0; count -= shiftAmount)
			{
				if (count < shiftAmount)
				{
					shiftAmount = count;
				}

				ulong carry = 0;
				for (int i = 0; i < temporary.Count; i++)
				{
					ulong val = ((ulong)temporary[i]) << shiftAmount;
					val |= carry;

					temporary[i] = (DType)(val & DigitsArray.AllBits);
					carry = (val >> DigitsArray.DataSizeBits);
				}

				if (carry != 0)
				{
					temporary.Add(0);
					temporary[temporary.Count - 1] = (DType)carry;
				}
			}
			m_data = new DType[temporary.Count];
			temporary.CopyTo(m_data);
			return m_data.Length;
		}
	}
	#endregion

	/// <summary>
	/// Represents a integer of abitrary length.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A BigInteger object is immutable like System.String. The object can not be modifed, and new BigInteger objects are
	/// created by using the operations of existing BigInteger objects.
	/// </para>
	/// <para>
	/// Internally a BigInteger object is an array of ? that is represents the digits of the n-place integer. Negative BigIntegers
	/// are stored internally as 1's complements, thus every BigInteger object contains 1 or more padding elements to hold the sign.
	/// </para>
	/// </remarks>
	/// <example>
	/// <code>
	/// public class MainProgram
	/// {
	///		[STAThread]
	///		public static void Main(string[] args)
	///		{
	///			BigInteger a = new BigInteger(25);
	///			a = a + 100;
	///			
	///			BigInteger b = new BigInteger("139435810094598308945890230913");
	///			
	///			BigInteger c = b / a;
	///			BigInteger d = b % a;
	///			
	///			BigInteger e = (c * a) + d;
	///			if (e != b)
	///			{
	///				Console.WriteLine("Can never be true.");
	///			}
	///		}
	///	</code>
	/// </example>
	public class BigInteger
	{
		private DigitsArray m_digits;
		private int m_decimalPointIndex;
		private bool m_isDecimalPoint = false;

		#region Constructors
		/// <summary>
		/// Create a BigInteger with an integer value of 0.
		/// </summary>
		public BigInteger()
		{
			m_digits = new DigitsArray(1, 1);
		}

		public BigInteger(float number)
		{
			m_digits = new DigitsArray(1, 1);
			m_isDecimalPoint = true;
			m_decimalPointIndex = 0;

			// 소수점 계산
			string numberStr = number.ToString();
			int decimalPointIndex = numberStr.IndexOf("."); // 0.1
			if (decimalPointIndex == -1) // 소수점이 없는 경우
			{
				m_isDecimalPoint = false;
				Construct(number.ToString(), 10);
				return;
			}
			m_decimalPointIndex = numberStr.Length - decimalPointIndex - 1; // 1 = 3 - 2
			m_isDecimalPoint = true;
			numberStr = numberStr.Replace(".", ""); // 0.1 -> 01

			Construct(numberStr, 10);
		}

		/// <summary>
		/// Creates a BigInteger with the value of the operand.
		/// </summary>
		/// <param name="number">A long.</param>
		public BigInteger(long number)
		{
			m_digits = new DigitsArray((8 / DigitsArray.DataSizeOf) + 1, 0);
			while (number != 0 && m_digits.DataUsed < m_digits.Count)
			{
				m_digits[m_digits.DataUsed] = (DType)(number & DigitsArray.AllBits);
				number >>= DigitsArray.DataSizeBits;
				m_digits.DataUsed++;
			}
			m_digits.ResetDataUsed();
		}

		/// <summary>
		/// Creates a BigInteger with the value of the operand. Can never be negative.
		/// </summary>
		/// <param name="number">A unsigned long.</param>
		public BigInteger(ulong number)
		{
			m_digits = new DigitsArray((8 / DigitsArray.DataSizeOf) + 1, 0);
			while (number != 0 && m_digits.DataUsed < m_digits.Count)
			{
				m_digits[m_digits.DataUsed] = (DType)(number & DigitsArray.AllBits);
				number >>= DigitsArray.DataSizeBits;
				m_digits.DataUsed++;
			}
			m_digits.ResetDataUsed();
		}

		/// <summary>
		/// Creates a BigInteger initialized from the byte array.
		/// </summary>
		/// <param name="array"></param>
		public BigInteger(byte[] array)
		{
			ConstructFrom(array, 0, array.Length);
		}

		/// <summary>
		/// Creates a BigInteger initialized from the byte array ending at <paramref name="length" />.
		/// </summary>
		/// <param name="array">A byte array.</param>
		/// <param name="length">Int number of bytes to use.</param>
		public BigInteger(byte[] array, int length)
		{
			ConstructFrom(array, 0, length);
		}

		/// <summary>
		/// Creates a BigInteger initialized from <paramref name="length" /> bytes starting at <paramref name="offset" />.
		/// </summary>
		/// <param name="array">A byte array.</param>
		/// <param name="offset">Int offset into the <paramref name="array" />.</param>
		/// <param name="length">Int number of bytes.</param>
		public BigInteger(byte[] array, int offset, int length)
		{
			ConstructFrom(array, offset, length);
		}

		private void ConstructFrom(byte[] array, int offset, int length)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset > array.Length || length > array.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length > array.Length || (offset + length) > array.Length)
			{
				throw new ArgumentOutOfRangeException("length");
			}

			int estSize = length / 4;
			int leftOver = length & 3;
			if (leftOver != 0)
			{
				++estSize;
			}

			m_digits = new DigitsArray(estSize + 1, 0); // alloc one extra since we can't init -'s from here.

			for (int i = offset + length - 1, j = 0; (i - offset) >= 3; i -= 4, j++)
			{
				m_digits[j] = (DType)((array[i - 3] << 24) + (array[i - 2] << 16) + (array[i - 1] << 8) + array[i]);
				m_digits.DataUsed++;
			}

			DType accumulator = 0;
			for (int i = leftOver; i > 0; i--)
			{
				DType digit = array[offset + leftOver - i];
				digit = (digit << ((i - 1) * 8));
				accumulator |= digit;
			}
			m_digits[m_digits.DataUsed] = accumulator;

			m_digits.ResetDataUsed();
		}

		/// <summary>
		/// Creates a BigInteger in base-10 from the parameter.
		/// </summary>
		/// <remarks>
		///  The new BigInteger is negative if the <paramref name="digits" /> has a leading - (minus).
		/// </remarks>
		/// <param name="digits">A string</param>
		public BigInteger(string digits)
		{
			Construct(digits, 10);
		}

		/// <summary>
		/// Creates a BigInteger in base and value from the parameters.
		/// </summary>
		/// <remarks>
		///  The new BigInteger is negative if the <paramref name="digits" /> has a leading - (minus).
		/// </remarks>
		/// <param name="digits">A string</param>
		/// <param name="radix">A int between 2 and 36.</param>
		public BigInteger(string digits, int radix)
		{
			Construct(digits, radix);
		}

		private void Construct(string digits, int radix)
		{
			if (string.IsNullOrEmpty(digits))
			{
				throw new ArgumentNullException("digits");
			}
			// 소수점 계산
			if (digits.Split(".").Length > 1)
			{
				Debug.Log("소수점 계산 " + digits);
				int decimalPointIndex = digits.IndexOf("."); // 0.1
				m_decimalPointIndex = digits.Length - decimalPointIndex - 1; // 1 = 3 - 2
				m_isDecimalPoint = true;
				digits = digits.Replace(".", ""); // 0.1 -> 01
			}

			BigInteger multiplier = new BigInteger(1);
			BigInteger result = new BigInteger();
			digits = digits.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();
			if (digits.Length == 0)
			{
				throw new ArgumentException("digits cannot be an empty string after trim.", nameof(digits));
			}
			int nDigits = digits[0] == '-' ? 1 : 0;

			for (int idx = digits.Length - 1; idx >= nDigits; idx--)
			{
				int d = (int)digits[idx];
				if (d >= '0' && d <= '9')
				{
					d -= '0';
				}
				else if (d >= 'A' && d <= 'Z')
				{
					d = (d - 'A') + 10;
				}
				else
				{
					throw new ArgumentOutOfRangeException("digits", digits);
				}

				if (d >= radix)
				{
					throw new ArgumentOutOfRangeException("digits");
				}
				result += (multiplier * d);
				multiplier *= radix;
			}

			if (digits[0] == '-')
			{
				result = -result;
			}

			this.m_digits = result.m_digits;

			if (m_isDecimalPoint)
				Debug.Log("Biginteger !!! " + this.m_digits.ToString());
		}

		/// <summary>
		/// Copy constructor, doesn't copy the digits parameter, assumes <code>this</code> owns the DigitsArray.
		/// </summary>
		/// <remarks>The <paramef name="digits" /> parameter is saved and reset.</remarks>
		/// <param name="digits"></param>
		private BigInteger(DigitsArray digits)
		{
			digits.ResetDataUsed();
			this.m_digits = digits;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// A bool value that is true when the BigInteger is negative (less than zero).
		/// </summary>
		/// <value>
		/// A bool value that is true when the BigInteger is negative (less than zero).
		/// </value>
		public bool IsNegative { get { return m_digits.IsNegative; } }

		/// <summary>
		/// A bool value that is true when the BigInteger is exactly zero.
		/// </summary>
		/// <value>
		/// A bool value that is true when the BigInteger is exactly zero.
		/// </value>
		public bool IsZero { get { return m_digits.IsZero; } }
		#endregion

		#region Implicit Type Operators Overloads

		/// <summary>
		/// Creates a BigInteger from a long.
		/// </summary>
		/// <param name="value">A long.</param>
		/// <returns>A BigInteger initialzed by <paramref name="value" />.</returns>
		public static implicit operator BigInteger(long value)
		{
			return (new BigInteger(value));
		}

		/// <summary>
		/// Creates a BigInteger from a ulong.
		/// </summary>
		/// <param name="value">A ulong.</param>
		/// <returns>A BigInteger initialzed by <paramref name="value" />.</returns>
		public static implicit operator BigInteger(ulong value)
		{
			return (new BigInteger(value));
		}

		/// <summary>
		/// Creates a BigInteger from a int.
		/// </summary>
		/// <param name="value">A int.</param>
		/// <returns>A BigInteger initialzed by <paramref name="value" />.</returns>
		public static implicit operator BigInteger(int value)
		{
			return (new BigInteger((long)value));
		}

		/// <summary>
		/// Creates a BigInteger from a uint.
		/// </summary>
		/// <param name="value">A uint.</param>
		/// <returns>A BigInteger initialzed by <paramref name="value" />.</returns>
		public static implicit operator BigInteger(uint value)
		{
			return (new BigInteger((ulong)value));
		}
		#endregion

		#region Addition and Subtraction Operator Overloads
		/// <summary>
		/// Adds two BigIntegers and returns a new BigInteger that represents the sum.
		/// </summary>
		/// <param name="leftSide">A BigInteger</param>
		/// <param name="rightSide">A BigInteger</param>
		/// <returns>The BigInteger result of adding <paramref name="leftSide" /> and <paramref name="rightSide" />.</returns>
		public static BigInteger operator +(BigInteger leftSide, BigInteger rightSide)
		{
			if (leftSide.m_isDecimalPoint || rightSide.m_isDecimalPoint)
			{
				// Debug.Log("소수점 연산!!");
				float leftFloat = leftSide.ToFloat();
				float rightFloat = rightSide.ToFloat();

				return new BigInteger((leftFloat + rightFloat).ToString());
			}

			// Debug.Log($"뭐가 없을까? {leftSide == null} {rightSide == null}");
			int size = Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(size + 1);

			long carry = 0;
			for (int i = 0; i < da.Count; i++)
			{
				long sum = (long)leftSide.m_digits[i] + (long)rightSide.m_digits[i] + carry;
				carry = (long)(sum >> DigitsArray.DataSizeBits);
				da[i] = (DType)(sum & DigitsArray.AllBits);
			}

			return new BigInteger(da);
		}

		// 소수점 정렬을 위한 0의 패딩 추가 메소드
		// private static BigInteger PadWithZeros(BigInteger value, int zerosCount)
		// {
		// 	// 여기에 value의 앞부분에 zerosCount만큼 0을 추가하는 로직 구현
		// 	// 예를 들어, 소수점을 처리하는 경우, 소수점 이하의 숫자 앞에 0을 추가해야 할 수 있습니다.
		// 	// 실제 구현은 BigInteger 객체의 내부 구조에 따라 달라질 것입니다.

		// 	// 가상의 구현 예시:
		// 	string paddedNumber = new string('0', zerosCount) + value.ToString();
		// 	BigInteger newValue = new BigInteger(paddedNumber);
		// 	newValue.m_decimalPointIndex = value.m_decimalPointIndex + zerosCount;
		// 	newValue.m_isDecimalPoint = value.m_isDecimalPoint;

		// 	Debug.Log("어디보자이이이 " + value + " / " + zerosCount + " / " + newValue);

		// 	return newValue;
		// }


		/// <summary>
		/// Adds two BigIntegers and returns a new BigInteger that represents the sum.
		/// </summary>
		/// <param name="leftSide">A BigInteger</param>
		/// <param name="rightSide">A BigInteger</param>
		/// <returns>The BigInteger result of adding <paramref name="leftSide" /> and <paramref name="rightSide" />.</returns>
		public static BigInteger Add(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide + rightSide;
		}

		/// <summary>
		/// Increments the BigInteger operand by 1.
		/// </summary>
		/// <param name="leftSide">The BigInteger operand.</param>
		/// <returns>The value of <paramref name="leftSide" /> incremented by 1.</returns>
		public static BigInteger operator ++(BigInteger leftSide)
		{
			return (leftSide + 1);
		}

		/// <summary>
		/// Increments the BigInteger operand by 1.
		/// </summary>
		/// <param name="leftSide">The BigInteger operand.</param>
		/// <returns>The value of <paramref name="leftSide" /> incremented by 1.</returns>
		public static BigInteger Increment(BigInteger leftSide)
		{
			return (leftSide + 1);
		}

		/// <summary>
		/// Substracts two BigIntegers and returns a new BigInteger that represents the sum.
		/// </summary>
		/// <param name="leftSide">A BigInteger</param>
		/// <param name="rightSide">A BigInteger</param>
		/// <returns>The BigInteger result of substracting <paramref name="leftSide" /> and <paramref name="rightSide" />.</returns>
		public static BigInteger operator -(BigInteger leftSide, BigInteger rightSide)
		{
			if (leftSide.m_isDecimalPoint || rightSide.m_isDecimalPoint)
			{
				// Debug.Log("소수점 연산!!");
				float leftFloat = leftSide.ToFloat();
				float rightFloat = rightSide.ToFloat();

				return new BigInteger((leftFloat - rightFloat).ToString());
			}


			int size = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed) + 1;
			DigitsArray da = new DigitsArray(size);

			long carry = 0;
			for (int i = 0; i < da.Count; i++)
			{
				long diff = (long)leftSide.m_digits[i] - (long)rightSide.m_digits[i] - carry;
				da[i] = (DType)(diff & DigitsArray.AllBits);
				da.DataUsed++;
				carry = ((diff < 0) ? 1 : 0);
			}
			return new BigInteger(da);
		}

		/// <summary>
		/// Substracts two BigIntegers and returns a new BigInteger that represents the sum.
		/// </summary>
		/// <param name="leftSide">A BigInteger</param>
		/// <param name="rightSide">A BigInteger</param>
		/// <returns>The BigInteger result of substracting <paramref name="leftSide" /> and <paramref name="rightSide" />.</returns>
		public static BigInteger Subtract(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide - rightSide;
		}

		/// <summary>
		/// Decrements the BigInteger operand by 1.
		/// </summary>
		/// <param name="leftSide">The BigInteger operand.</param>
		/// <returns>The value of the <paramref name="leftSide" /> decremented by 1.</returns>
		public static BigInteger operator --(BigInteger leftSide)
		{
			return (leftSide - 1);
		}

		/// <summary>
		/// Decrements the BigInteger operand by 1.
		/// </summary>
		/// <param name="leftSide">The BigInteger operand.</param>
		/// <returns>The value of the <paramref name="leftSide" /> decremented by 1.</returns>
		public static BigInteger Decrement(BigInteger leftSide)
		{
			return (leftSide - 1);
		}
		#endregion

		#region Negate Operator Overload
		/// <summary>
		/// Negates the BigInteger, that is, if the BigInteger is negative return a positive BigInteger, and if the
		/// BigInteger is negative return the postive.
		/// </summary>
		/// <param name="leftSide">A BigInteger operand.</param>
		/// <returns>The value of the <paramref name="this" /> negated.</returns>
		public static BigInteger operator -(BigInteger leftSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}

			if (leftSide.IsZero)
			{
				return new BigInteger(0);
			}

			DigitsArray da = new DigitsArray(leftSide.m_digits.DataUsed + 1, leftSide.m_digits.DataUsed + 1);

			for (int i = 0; i < da.Count; i++)
			{
				da[i] = (DType)(~(leftSide.m_digits[i]));
			}

			// add one to result (1's complement + 1)
			bool carry = true;
			int index = 0;
			while (carry && index < da.Count)
			{
				long val = (long)da[index] + 1;
				da[index] = (DType)(val & DigitsArray.AllBits);
				carry = ((val >> DigitsArray.DataSizeBits) > 0);
				index++;
			}

			return new BigInteger(da);
		}

		/// <summary>
		/// Negates the BigInteger, that is, if the BigInteger is negative return a positive BigInteger, and if the
		/// BigInteger is negative return the postive.
		/// </summary>
		/// <returns>The value of the <paramref name="this" /> negated.</returns>
		public BigInteger Negate()
		{
			return -this;
		}

		/// <summary>
		/// Creates a BigInteger absolute value of the operand.
		/// </summary>
		/// <param name="leftSide">A BigInteger.</param>
		/// <returns>A BigInteger that represents the absolute value of <paramref name="leftSide" />.</returns>
		public static BigInteger Abs(BigInteger leftSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (leftSide.IsNegative)
			{
				return -leftSide;
			}
			return leftSide;
		}
		#endregion

		#region Multiplication, Division and Modulus Operators
		/// <summary>
		/// Multiply two BigIntegers returning the result.
		/// </summary>
		/// <remarks>
		/// See Knuth.
		/// </remarks>
		/// <param name="leftSide">A BigInteger.</param>
		/// <param name="rightSide">A BigInteger</param>
		/// <returns></returns>
		public static BigInteger operator *(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}
			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}

			bool leftSideNeg = leftSide.IsNegative;
			bool rightSideNeg = rightSide.IsNegative;

			leftSide = Abs(leftSide);
			rightSide = Abs(rightSide);

			DigitsArray da = new DigitsArray(leftSide.m_digits.DataUsed + rightSide.m_digits.DataUsed);
			da.DataUsed = da.Count;

			for (int i = 0; i < leftSide.m_digits.DataUsed; i++)
			{
				ulong carry = 0;
				for (int j = 0, k = i; j < rightSide.m_digits.DataUsed; j++, k++)
				{
					ulong val = ((ulong)leftSide.m_digits[i] * (ulong)rightSide.m_digits[j]) + (ulong)da[k] + carry;

					da[k] = (DType)(val & DigitsArray.AllBits);
					carry = (val >> DigitsArray.DataSizeBits);
				}

				if (carry != 0)
				{
					da[i + rightSide.m_digits.DataUsed] = (DType)carry;
				}
			}

			//da.ResetDataUsed();
			BigInteger result = new BigInteger(da);
			return (leftSideNeg != rightSideNeg ? -result : result);
		}

		public static BigInteger Multiply(BigInteger leftSide, float rightSideFloat)
		{
			// float 값을 문자열로 변환
			string rightSideStr = rightSideFloat.ToString(System.Globalization.CultureInfo.InvariantCulture);

			// 문자열에서 소수점 위치 찾기
			int decimalPointIndex = rightSideStr.IndexOf('.');
			BigInteger rightSideBigInt;

			if (decimalPointIndex != -1)
			{
				// 소수점이 있는 경우, 소수점을 제거하고 BigInteger로 변환
				string integralPart = rightSideStr.Replace(".", "");
				rightSideBigInt = new BigInteger(integralPart);

				// 소수점 아래 자릿수만큼 결과값을 조정
				int decimalPlaces = rightSideStr.Length - decimalPointIndex - 1;
				BigInteger divisor = BigInteger.Pow(10, decimalPlaces);
				rightSideBigInt = rightSideBigInt / divisor;
			}
			else
			{
				// 소수점이 없는 경우, 직접 변환
				rightSideBigInt = new BigInteger(rightSideStr);
			}

			// BigInteger 간의 곱셈
			BigInteger result = leftSide * rightSideBigInt;

			return result;
		}


		/// <summary>
		/// Multiply two BigIntegers returning the result.
		/// </summary>
		/// <param name="leftSide">A BigInteger.</param>
		/// <param name="rightSide">A BigInteger</param>
		/// <returns></returns>
		public static BigInteger Multiply(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide * rightSide;
		}

		/// <summary>
		/// Divide a BigInteger by another BigInteger and returning the result.
		/// </summary>
		/// <param name="leftSide">A BigInteger divisor.</param>
		/// <param name="rightSide">A BigInteger dividend.</param>
		/// <returns>The BigInteger result.</returns>
		public static BigInteger operator /(BigInteger leftSide, BigInteger rightSide)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}
			if (rightSide == null)
			{
				throw new ArgumentNullException("rightSide");
			}

			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}

			bool divisorNeg = rightSide.IsNegative;
			bool dividendNeg = leftSide.IsNegative;

			leftSide = Abs(leftSide);
			rightSide = Abs(rightSide);

			if (leftSide < rightSide)
			{
				return new BigInteger(0);
			}

			BigInteger quotient;
			BigInteger remainder;
			Divide(leftSide, rightSide, out quotient, out remainder);

			return (dividendNeg != divisorNeg ? -quotient : quotient);
		}

		/// <summary>
		/// Divide a BigInteger by another BigInteger and returning the result.
		/// </summary>
		/// <param name="leftSide">A BigInteger divisor.</param>
		/// <param name="rightSide">A BigInteger dividend.</param>
		/// <returns>The BigInteger result.</returns>
		public static BigInteger Divide(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide / rightSide;
		}

		private static void Divide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (leftSide.IsZero)
			{
				quotient = new BigInteger();
				remainder = new BigInteger();
				return;
			}

			if (rightSide.m_digits.DataUsed == 1)
			{
				SingleDivide(leftSide, rightSide, out quotient, out remainder);
			}
			else
			{
				MultiDivide(leftSide, rightSide, out quotient, out remainder);
			}
		}

		private static void MultiDivide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}

			DType val = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
			int d = 0;
			for (uint mask = DigitsArray.HiBitSet; mask != 0 && (val & mask) == 0; mask >>= 1)
			{
				d++;
			}

			int remainderLen = leftSide.m_digits.DataUsed + 1;
			DType[] remainderDat = new DType[remainderLen];
			leftSide.m_digits.CopyTo(remainderDat, 0, leftSide.m_digits.DataUsed);

			DigitsArray.ShiftLeft(remainderDat, d);
			rightSide = rightSide << d;

			ulong firstDivisor = rightSide.m_digits[rightSide.m_digits.DataUsed - 1];
			ulong secondDivisor = (rightSide.m_digits.DataUsed < 2 ? (DType)0 : rightSide.m_digits[rightSide.m_digits.DataUsed - 2]);

			int divisorLen = rightSide.m_digits.DataUsed + 1;
			DigitsArray dividendPart = new DigitsArray(divisorLen, divisorLen);
			DType[] result = new DType[leftSide.m_digits.Count + 1];
			int resultPos = 0;

			ulong carryBit = (ulong)0x1 << DigitsArray.DataSizeBits; // 0x100000000
			for (int j = remainderLen - rightSide.m_digits.DataUsed, pos = remainderLen - 1; j > 0; j--, pos--)
			{
				ulong dividend = ((ulong)remainderDat[pos] << DigitsArray.DataSizeBits) + (ulong)remainderDat[pos - 1];
				ulong qHat = (dividend / firstDivisor);
				ulong rHat = (dividend % firstDivisor);

				while (pos >= 2)
				{
					if (qHat == carryBit || (qHat * secondDivisor) > ((rHat << DigitsArray.DataSizeBits) + remainderDat[pos - 2]))
					{
						qHat--;
						rHat += firstDivisor;
						if (rHat < carryBit)
						{
							continue;
						}
					}
					break;
				}

				for (int h = 0; h < divisorLen; h++)
				{
					dividendPart[divisorLen - h - 1] = remainderDat[pos - h];
				}

				BigInteger dTemp = new BigInteger(dividendPart);
				BigInteger rTemp = rightSide * (long)qHat;
				while (rTemp > dTemp)
				{
					qHat--;
					rTemp -= rightSide;
				}

				rTemp = dTemp - rTemp;
				for (int h = 0; h < divisorLen; h++)
				{
					remainderDat[pos - h] = rTemp.m_digits[rightSide.m_digits.DataUsed - h];
				}

				result[resultPos++] = (DType)qHat;
			}

			Array.Reverse(result, 0, resultPos);
			quotient = new BigInteger(new DigitsArray(result));

			int n = DigitsArray.ShiftRight(remainderDat, d);
			DigitsArray rDA = new DigitsArray(n, n);
			rDA.CopyFrom(remainderDat, 0, 0, rDA.DataUsed);
			remainder = new BigInteger(rDA);
		}

		private static void SingleDivide(BigInteger leftSide, BigInteger rightSide, out BigInteger quotient, out BigInteger remainder)
		{
			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}

			DigitsArray remainderDigits = new DigitsArray(leftSide.m_digits);
			remainderDigits.ResetDataUsed();

			int pos = remainderDigits.DataUsed - 1;
			ulong divisor = (ulong)rightSide.m_digits[0];
			ulong dividend = (ulong)remainderDigits[pos];

			DType[] result = new DType[leftSide.m_digits.Count];
			leftSide.m_digits.CopyTo(result, 0, result.Length);
			int resultPos = 0;

			if (dividend >= divisor)
			{
				result[resultPos++] = (DType)(dividend / divisor);
				remainderDigits[pos] = (DType)(dividend % divisor);
			}
			pos--;

			while (pos >= 0)
			{
				dividend = ((ulong)(remainderDigits[pos + 1]) << DigitsArray.DataSizeBits) + (ulong)remainderDigits[pos];
				result[resultPos++] = (DType)(dividend / divisor);
				remainderDigits[pos + 1] = 0;
				remainderDigits[pos--] = (DType)(dividend % divisor);
			}
			remainder = new BigInteger(remainderDigits);

			DigitsArray quotientDigits = new DigitsArray(resultPos + 1, resultPos);
			int j = 0;
			for (int i = quotientDigits.DataUsed - 1; i >= 0; i--, j++)
			{
				quotientDigits[j] = result[i];
			}
			quotient = new BigInteger(quotientDigits);
		}

		/// <summary>
		/// Perform the modulus of a BigInteger with another BigInteger and return the result.
		/// </summary>
		/// <param name="leftSide">A BigInteger divisor.</param>
		/// <param name="rightSide">A BigInteger dividend.</param>
		/// <returns>The BigInteger result.</returns>
		public static BigInteger operator %(BigInteger leftSide, BigInteger rightSide)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			if (rightSide == null)
			{
				throw new ArgumentNullException("rightSide");
			}

			if (rightSide.IsZero)
			{
				throw new DivideByZeroException();
			}

			BigInteger quotient;
			BigInteger remainder;

			bool dividendNeg = leftSide.IsNegative;
			leftSide = Abs(leftSide);
			rightSide = Abs(rightSide);

			if (leftSide < rightSide)
			{
				return leftSide;
			}

			Divide(leftSide, rightSide, out quotient, out remainder);

			return (dividendNeg ? -remainder : remainder);
		}

		/// <summary>
		/// Perform the modulus of a BigInteger with another BigInteger and return the result.
		/// </summary>
		/// <param name="leftSide">A BigInteger divisor.</param>
		/// <param name="rightSide">A BigInteger dividend.</param>
		/// <returns>The BigInteger result.</returns>
		public static BigInteger Modulus(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide % rightSide;
		}

		#endregion

		#region Exponentiation

		public BigInteger Pow(BigInteger power)
		{
			return Pow(this, power);
		}

		/// <summary>
		/// Returns a base number raised to a specified power. Currently only positive (>= 0) 
		/// exponents allowed.
		/// </summary>
		/// <param name="b">A BigInteger to be raised to a power.</param>
		/// <param name="power">A BigInteger that specifies the power.</param>
		public static BigInteger Pow(BigInteger b, BigInteger power)
		{

			if (b == null)
			{
				throw new ArgumentNullException("b");
			}

			if (power == null)
			{
				throw new ArgumentNullException("power");
			}

			if (power < 0)
			{
				throw new ArgumentOutOfRangeException("power", "Currently negative exponents are not supported");
			}


			BigInteger result = 1;
			while (power != 0)
			{

				if ((power & 1) != 0)
					result *= b;
				power >>= 1;
				b *= b;
			}

			return result;
		}

		#endregion

		#region Bitwise Operator Overloads

		public static BigInteger operator &(BigInteger leftSide, BigInteger rightSide)
		{
			int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(len, len);
			for (int idx = 0; idx < len; idx++)
			{
				da[idx] = (DType)(leftSide.m_digits[idx] & rightSide.m_digits[idx]);
			}
			return new BigInteger(da);
		}

		public static BigInteger BitwiseAnd(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide & rightSide;
		}

		public static BigInteger operator |(BigInteger leftSide, BigInteger rightSide)
		{
			int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(len, len);
			for (int idx = 0; idx < len; idx++)
			{
				da[idx] = (DType)(leftSide.m_digits[idx] | rightSide.m_digits[idx]);
			}
			return new BigInteger(da);
		}

		public static BigInteger BitwiseOr(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide | rightSide;
		}

		public static BigInteger operator ^(BigInteger leftSide, BigInteger rightSide)
		{
			int len = System.Math.Max(leftSide.m_digits.DataUsed, rightSide.m_digits.DataUsed);
			DigitsArray da = new DigitsArray(len, len);
			for (int idx = 0; idx < len; idx++)
			{
				da[idx] = (DType)(leftSide.m_digits[idx] ^ rightSide.m_digits[idx]);
			}
			return new BigInteger(da);
		}

		public static BigInteger Xor(BigInteger leftSide, BigInteger rightSide)
		{
			return leftSide ^ rightSide;
		}

		public static BigInteger operator ~(BigInteger leftSide)
		{
			DigitsArray da = new DigitsArray(leftSide.m_digits.Count);
			for (int idx = 0; idx < da.Count; idx++)
			{
				da[idx] = (DType)(~(leftSide.m_digits[idx]));
			}

			return new BigInteger(da);
		}

		public static BigInteger OnesComplement(BigInteger leftSide)
		{
			return ~leftSide;
		}

		#endregion

		#region Left and Right Shift Operator Overloads
		public static BigInteger operator <<(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			DigitsArray da = new DigitsArray(leftSide.m_digits);
			da.DataUsed = da.ShiftLeftWithoutOverflow(shiftCount);

			return new BigInteger(da);
		}

		public static BigInteger LeftShift(BigInteger leftSide, int shiftCount)
		{
			return leftSide << shiftCount;
		}

		public static BigInteger operator >>(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			DigitsArray da = new DigitsArray(leftSide.m_digits);
			da.DataUsed = da.ShiftRight(shiftCount);

			if (leftSide.IsNegative)
			{
				for (int i = da.Count - 1; i >= da.DataUsed; i--)
				{
					da[i] = DigitsArray.AllBits;
				}

				DType mask = DigitsArray.HiBitSet;
				for (int i = 0; i < DigitsArray.DataSizeBits; i++)
				{
					if ((da[da.DataUsed - 1] & mask) == DigitsArray.HiBitSet)
					{
						break;
					}
					da[da.DataUsed - 1] |= mask;
					mask >>= 1;
				}
				da.DataUsed = da.Count;
			}

			return new BigInteger(da);
		}

		public static BigInteger RightShift(BigInteger leftSide, int shiftCount)
		{
			if (leftSide == null)
			{
				throw new ArgumentNullException("leftSide");
			}

			return leftSide >> shiftCount;
		}
		#endregion

		#region Relational Operator Overloads

		/// <summary>
		/// Compare this instance to a specified object and returns indication of their relative value.
		/// </summary>
		/// <param name="value">An object to compare, or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>A signed number indicating the relative value of this instance and <i>value</i>.
		/// <list type="table">
		///		<listheader>
		///			<term>Return Value</term>
		///			<description>Description</description>
		///		</listheader>
		///		<item>
		///			<term>Less than zero</term>
		///			<description>This instance is less than <i>value</i>.</description>
		///		</item>
		///		<item>
		///			<term>Zero</term>
		///			<description>This instance is equal to <i>value</i>.</description>
		///		</item>
		///		<item>
		///			<term>Greater than zero</term>
		///			<description>
		///				This instance is greater than <i>value</i>. 
		///				<para>-or-</para>
		///				<i>value</i> is a null reference (<b>Nothing</b> in Visual Basic).
		///			</description>
		///		</item>
		/// </list>
		/// </returns>
		public int CompareTo(BigInteger value)
		{
			return Compare(this, value);
		}

		/// <summary>
		/// Compare two objects and return an indication of their relative value.
		/// </summary>
		/// <param name="leftSide">An object to compare, or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <param name="rightSide">An object to compare, or a null reference (<b>Nothing</b> in Visual Basic).</param>
		/// <returns>A signed number indicating the relative value of this instance and <i>value</i>.
		/// <list type="table">
		///		<listheader>
		///			<term>Return Value</term>
		///			<description>Description</description>
		///		</listheader>
		///		<item>
		///			<term>Less than zero</term>
		///			<description>This instance is less than <i>value</i>.</description>
		///		</item>
		///		<item>
		///			<term>Zero</term>
		///			<description>This instance is equal to <i>value</i>.</description>
		///		</item>
		///		<item>
		///			<term>Greater than zero</term>
		///			<description>
		///				This instance is greater than <i>value</i>. 
		///				<para>-or-</para>
		///				<i>value</i> is a null reference (<b>Nothing</b> in Visual Basic).
		///			</description>
		///		</item>
		/// </list>
		/// </returns>
		public static int Compare(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, rightSide))
			{
				return 0;
			}

			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}

			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}

			if (leftSide > rightSide) return 1;
			if (leftSide == rightSide) return 0;
			return -1;
		}

		public static bool operator ==(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, rightSide))
			{
				return true;
			}

			if (object.ReferenceEquals(leftSide, null) || object.ReferenceEquals(rightSide, null))
			{
				return false;
			}

			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return false;
			}

			return leftSide.Equals(rightSide);
		}

		public static bool operator !=(BigInteger leftSide, BigInteger rightSide)
		{
			return !(leftSide == rightSide);
		}

		public static bool operator >(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}

			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}

			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return rightSide.IsNegative;
			}

			if (leftSide.m_digits.DataUsed != rightSide.m_digits.DataUsed)
			{
				return leftSide.m_digits.DataUsed > rightSide.m_digits.DataUsed;
			}

			for (int idx = leftSide.m_digits.DataUsed - 1; idx >= 0; idx--)
			{
				if (leftSide.m_digits[idx] != rightSide.m_digits[idx])
				{
					return (leftSide.m_digits[idx] > rightSide.m_digits[idx]);
				}
			}
			return false;
		}

		public static bool operator <(BigInteger leftSide, BigInteger rightSide)
		{
			if (object.ReferenceEquals(leftSide, null))
			{
				throw new ArgumentNullException("leftSide");
			}

			if (object.ReferenceEquals(rightSide, null))
			{
				throw new ArgumentNullException("rightSide");
			}

			if (leftSide.IsNegative != rightSide.IsNegative)
			{
				return leftSide.IsNegative;
			}

			if (leftSide.m_digits.DataUsed != rightSide.m_digits.DataUsed)
			{
				return leftSide.m_digits.DataUsed < rightSide.m_digits.DataUsed;
			}

			for (int idx = leftSide.m_digits.DataUsed - 1; idx >= 0; idx--)
			{
				if (leftSide.m_digits[idx] != rightSide.m_digits[idx])
				{
					return (leftSide.m_digits[idx] < rightSide.m_digits[idx]);
				}
			}
			return false;
		}

		public static bool operator >=(BigInteger leftSide, BigInteger rightSide)
		{
			return Compare(leftSide, rightSide) >= 0;
		}

		public static bool operator <=(BigInteger leftSide, BigInteger rightSide)
		{
			return Compare(leftSide, rightSide) <= 0;
		}
		#endregion

		#region Object Overrides
		/// <summary>
		/// Determines whether two Object instances are equal.
		/// </summary>
		/// <param name="obj">An <see cref="System.Object">Object</see> to compare with this instance.</param>
		/// <returns></returns>
		/// <seealso cref="System.Object">System.Object</seealso> 
		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, null))
			{
				return false;
			}

			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}

			BigInteger c = (BigInteger)obj;
			if (this.m_digits.DataUsed != c.m_digits.DataUsed)
			{
				return false;
			}

			for (int idx = 0; idx < this.m_digits.DataUsed; idx++)
			{
				if (this.m_digits[idx] != c.m_digits[idx])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer has code.</returns>
		/// <seealso cref="System.Object">System.Object</seealso> 
		public override int GetHashCode()
		{
			return this.m_digits.GetHashCode();
		}

		/// <summary>
		///  Returns the array entries as a string. Used for debugging.
		/// </summary>
		/// <returns>The data as string.</returns>
		public string GetDataAsString()
		{
			return this.m_digits.GetDataAsString();
		}

		/// <summary>
		/// Converts the numeric value of this instance to its equivalent base 10 string representation.
		/// </summary>
		/// <returns>A <see cref="System.String">String</see> in base 10.</returns>
		/// <seealso cref="System.Object">System.Object</seealso> 
		public override string ToString()
		{
			return ToString(10);
		}

		public float ToFloat()
		{
			return float.Parse(ToString());
		}

		public int ToInt()
		{
			return int.Parse(ToString());
		}

		#endregion

		#region Type Conversion Methods
		/// <summary>
		/// Converts the numeric value of this instance to its equivalent string representation in specified base.
		/// </summary>
		/// <param name="radix">Int radix between 2 and 36</param>
		/// <returns>A string.</returns>
		public string ToString(int radix)
		{
			if (radix < 2 || radix > 36)
			{
				throw new ArgumentOutOfRangeException("radix");
			}

			if (IsZero)
			{
				return "0";
			}

			BigInteger a = this;
			bool negative = a.IsNegative;
			a = Abs(this);

			BigInteger quotient;
			BigInteger remainder;
			BigInteger biRadix = new BigInteger(radix);

			const string charSet = "0123456789abcdefghijklmnopqrstuvwxyz";
			System.Collections.ArrayList al = new System.Collections.ArrayList();
			while (a.m_digits.DataUsed > 1 || (a.m_digits.DataUsed == 1 && a.m_digits[0] != 0))
			{
				Divide(a, biRadix, out quotient, out remainder);
				al.Insert(0, charSet[(int)remainder.m_digits[0]]);
				a = quotient;
			}

			string result = new String((char[])al.ToArray(typeof(char)));
			if (radix == 10 && negative)
			{
				return "-" + result;
			}
			if (m_isDecimalPoint)
			{
				// result의 길이가 m_decimalPointIndex보다 작은 경우, 필요한 만큼 0을 추가합니다.
				int zerosToAdd = m_decimalPointIndex - result.Length;
				for (int i = 0; i < zerosToAdd; i++)
				{
					result += "0"; // result의 끝에 0 추가
				}

				// 소수점을 삽입할 위치 계산 (리버스 인덱스)
				int insertIndex = result.Length - m_decimalPointIndex;
				if (insertIndex <= 0)
				{
					// insertIndex가 0 이하인 경우, 즉 소수점이 맨 앞에 와야하는 경우
					// "0."을 맨 앞에 추가하고 나머지 숫자를 붙입니다.
					result = "0." + result.PadLeft(zerosToAdd, '0');
				}
				else
				{
					// 그 외의 경우, 계산된 위치에 소수점을 삽입
					result = result.Insert(insertIndex, ".");
				}
			}


			return result;
		}

		/// <summary>
		/// Returns string in hexidecimal of the internal digit representation.
		/// </summary>
		/// <remarks>
		/// This is not the same as ToString(16). This method does not return the sign, but instead
		/// dumps the digits array into a string representation in base 16.
		/// </remarks>
		/// <returns>A string in base 16.</returns>
		public string ToHexString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendFormat("{0:X}", m_digits[m_digits.DataUsed - 1]);

			string f = "{0:X" + (2 * DigitsArray.DataSizeOf) + "}";
			for (int i = m_digits.DataUsed - 2; i >= 0; i--)
			{
				sb.AppendFormat(f, m_digits[i]);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns BigInteger as System.Int16 if possible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>Int value of BigInteger</returns>
		/// <exception cref="System.Exception">When BigInteger is too large to fit into System.Int16</exception>
		public static int ToInt16(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return System.Int16.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Returns BigInteger as System.UInt16 if possible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception">When BigInteger is too large to fit into System.UInt16</exception>
		public static uint ToUInt16(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return System.UInt16.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Returns BigInteger as System.Int32 if possible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception">When BigInteger is too large to fit into System.Int32</exception>
		public static int ToInt32(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return System.Int32.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Returns BigInteger as System.UInt32 if possible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception">When BigInteger is too large to fit into System.UInt32</exception>
		public static uint ToUInt32(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return System.UInt32.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Returns BigInteger as System.Int64 if possible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception">When BigInteger is too large to fit into System.Int64</exception>
		public static long ToInt64(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return System.Int64.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Returns BigInteger as System.UInt64 if possible.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="System.Exception">When BigInteger is too large to fit into System.UInt64</exception>
		public static ulong ToUInt64(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return System.UInt64.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		public static long ToInt64Safely(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return long.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		public static ulong ToUInt64Safely(BigInteger value)
		{
			if (object.ReferenceEquals(value, null))
			{
				throw new ArgumentNullException("value");
			}
			return ulong.Parse(value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture);
		}

		#endregion
		int index = 0;
		public string ChangeMoney()
		{
			string targetString = ToString();
			string[] unit = new string[] { "", "K", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
			"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
			"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
			"CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
			"DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
			"EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
			"FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ",
			"GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ",
			"HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ",
			"IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV", "IW", "IX", "IY", "IZ",
			"JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JI", "JJ", "JK", "JL", "JM", "JN", "JO", "JP", "JQ", "JR", "JS", "JT", "JU", "JV", "JW", "JX", "JY", "JZ",
			"KA", "KB", "KC", "KD", "KE", "KF", "KG", "KH", "KI", "KJ", "KK", "KL", "KM", "KN", "KO", "KP", "KQ", "KR", "KS", "KT", "KU", "KV", "KW", "KX", "KY", "KZ",
			"LA", "LB", "LC", "LD", "LE", "LF", "LG", "LH", "LI", "LJ", "LK", "LL", "LM", "LN", "LO", "LP", "LQ", "LR", "LS", "LT", "LU", "LV", "LW", "LX", "LY", "LZ",
			"MA", "MB", "MC", "MD", "ME", "MF", "MG", "MH", "MI", "MJ", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ",
			"NA", "NB", "NC", "ND", "NE", "NF", "NG", "NH", "NI", "NJ", "NK", "NL", "NM", "NN", "NO", "NP", "NQ", "NR", "NS", "NT", "NU", "NV", "NW", "NX", "NY", "NZ",
			"OA", "OB", "OC", "OD", "OE", "OF", "OG", "OH", "OI", "OJ", "OK", "OL", "OM", "ON", "OO", "OP", "OQ", "OR", "OS", "OT", "OU", "OV", "OW", "OX", "OY", "OZ",
			"PA", "PB", "PC", "PD", "PE", "PF", "PG", "PH", "PI", "PJ", "PK", "PL", "PM", "PN", "PO", "PP", "PQ", "PR", "PS", "PT", "PU", "PV", "PW", "PX", "PY", "PZ"
		};

			int[] cVal;

			if (index < unit.Length)
			{
				cVal = new int[unit.Length];
				index = 0;

				while (true)
				{
					string last4 = "";
					if (targetString.Length >= 4 && targetString[0] != '-')
					{
						last4 = targetString.Substring(targetString.Length - 4);
						int intLast4 = int.Parse(last4);

						cVal[index] = intLast4 % 1000;

						targetString = targetString.Remove(targetString.Length - 3);
					}
					else
					{
						cVal[index] = int.Parse(targetString);
						break;
					}
					index++;
				}
				if (index > 0)
				{
					int r = cVal[index] * 1000 + cVal[index - 1];
					int front = r / 1000;
					int back = r % 1000 / 100;
					return $"{front}.{back}{unit[index]}";
					// return string.Format("{0:#.#}{1}", MathF.Truncate(r / 1000f), unit[index]);
					//return string.Format("{0:N3}{1}", (float)r / 1000f, unit[index]);
				}
			}

			return targetString;
		}
		public static string ChangeMoney(string targetString)
		{
			int index = 0;
			string[] unit = new string[] { "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
			"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
			"BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
			"CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
			"DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
			"EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
			"FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ",
			"GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ",
			"HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ",
			"IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV", "IW", "IX", "IY", "IZ",
			"JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JI", "JJ", "JK", "JL", "JM", "JN", "JO", "JP", "JQ", "JR", "JS", "JT", "JU", "JV", "JW", "JX", "JY", "JZ",
			"KA", "KB", "KC", "KD", "KE", "KF", "KG", "KH", "KI", "KJ", "KK", "KL", "KM", "KN", "KO", "KP", "KQ", "KR", "KS", "KT", "KU", "KV", "KW", "KX", "KY", "KZ",
			"LA", "LB", "LC", "LD", "LE", "LF", "LG", "LH", "LI", "LJ", "LK", "LL", "LM", "LN", "LO", "LP", "LQ", "LR", "LS", "LT", "LU", "LV", "LW", "LX", "LY", "LZ",
			"MA", "MB", "MC", "MD", "ME", "MF", "MG", "MH", "MI", "MJ", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ",
			"NA", "NB", "NC", "ND", "NE", "NF", "NG", "NH", "NI", "NJ", "NK", "NL", "NM", "NN", "NO", "NP", "NQ", "NR", "NS", "NT", "NU", "NV", "NW", "NX", "NY", "NZ",
			"OA", "OB", "OC", "OD", "OE", "OF", "OG", "OH", "OI", "OJ", "OK", "OL", "OM", "ON", "OO", "OP", "OQ", "OR", "OS", "OT", "OU", "OV", "OW", "OX", "OY", "OZ",
			"PA", "PB", "PC", "PD", "PE", "PF", "PG", "PH", "PI", "PJ", "PK", "PL", "PM", "PN", "PO", "PP", "PQ", "PR", "PS", "PT", "PU", "PV", "PW", "PX", "PY", "PZ"
		};

			int[] cVal;

			if (index < unit.Length)
			{
				cVal = new int[unit.Length];
				index = 0;

				while (true)
				{
					string last4 = "";
					if (targetString.Length >= 4)
					{
						last4 = targetString.Substring(targetString.Length - 4);
						int intLast4 = int.Parse(last4);

						cVal[index] = intLast4 % 1000;

						targetString = targetString.Remove(targetString.Length - 3);
					}
					else
					{
						cVal[index] = int.Parse(targetString);
						break;
					}
					index++;
				}
				if (index > 0)
				{
					int r = cVal[index] * 1000 + cVal[index - 1];
					return string.Format("{0:#.#}{1}", (float)r / 1000f, unit[index]);
					//return string.Format("{0:N3}{1}", (float)r / 1000f, unit[index]);
				}
			}

			return targetString;
		}

		public static int CountDecimalPlaces(float value)
		{
			string strValue = value.ToString();
			int decimalPointIndex = strValue.IndexOf('.');
			if (decimalPointIndex == -1)
				return 0;
			return strValue.Length - decimalPointIndex - 1;
		}

	}
}
