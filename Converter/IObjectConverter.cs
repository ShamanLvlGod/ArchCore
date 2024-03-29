﻿namespace ArchCore.Converter {
	public interface IObjectConverter {
		T ToObject<T>(string data);
		string ToString(object obj);
		byte[] ToRawData(object obj);
	}
}