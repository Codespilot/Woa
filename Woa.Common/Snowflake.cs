namespace Woa.Common;

internal class Snowflake
{
	public const long Twepoch = 1288834974657L;

	private const int MachineIdBits = 5;
	private const int DatacenterIdBits = 5;
	private const int SequenceBits = 12;
	internal const long MaxMachineId = -1L ^ (-1L << MachineIdBits);
	internal const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

	private const int MachineIdShift = SequenceBits;
	private const int DatacenterIdShift = SequenceBits + MachineIdBits;
	public const int TimestampLeftShift = SequenceBits + MachineIdBits + DatacenterIdBits;
	private const long SequenceMask = -1L ^ (-1L << SequenceBits);

	private readonly object _lock = new();
	private long _lastTimestamp = -1L;

	private static readonly Lazy<Snowflake> instance = new(() => new Snowflake(0, 0));

	public static Snowflake Instance()
	{
		return instance.Value;
	}

	public Snowflake(long machineId, long datacenterId)
	{
		MachineId = machineId;
		DatacenterId = datacenterId;
	}

	/// <summary>
	/// 
	/// </summary>
	public long MachineId { get; } = 0;
	/// <summary>
	/// 
	/// </summary>
	public long DatacenterId { get; } = 0;
	/// <summary>
	/// 序列号
	/// </summary>
	public long Sequence { get; internal set; } = 0L;



	/// <summary>
	/// 生成下一个Id
	/// </summary>
	/// <returns>返回雪花Id</returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="Exception"></exception>
	public long NextId()
	{
		// sanity check for workerId
		if (MachineId > MaxMachineId || MachineId < 0)
			throw new ArgumentException($"worker Id can't be greater than {MaxMachineId} or less than 0");

		if (DatacenterId > MaxDatacenterId || DatacenterId < 0)
			throw new ArgumentException($"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");

		lock (_lock)
		{
			var timestamp = TimeGen();

			if (timestamp < _lastTimestamp)
				throw new Exception($"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");

			if (_lastTimestamp == timestamp)
			{
				Sequence = (Sequence + 1) & SequenceMask;
				if (Sequence == 0) timestamp = TilNextMillis(_lastTimestamp);
			}
			else
			{
				Sequence = 0;
			}

			_lastTimestamp = timestamp;
			var id = ((timestamp - Twepoch) << TimestampLeftShift) |
					 (DatacenterId << DatacenterIdShift) |
					 (MachineId << MachineIdShift) | Sequence;

			return id;
		}
	}

	private long TilNextMillis(long lastTimestamp)
	{
		var timestamp = TimeGen();
		while (timestamp <= lastTimestamp) timestamp = TimeGen();
		return timestamp;
	}

	private long TimeGen() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
