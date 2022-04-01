/////////////////////////////////////////
//	2013-07-22		: NGUI Object Destroy
//	2013-07-26		: getRandSerial(Ticks이용)
//					  encoding <=> decoding
/////////////////////////////////////////

//#define DEBUG_ON
using UnityEngine;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

public class SSUtil{
	public static System.Text.Encoding enc = System.Text.Encoding.ASCII;
	public static string strkey				= "secret8";
		
	public static void testEncoding(){		
		//Encrypted string: p+nsujMopWLrTskX5x4hXqWnx5yNduJccwq/vdwjqsX4Dk2p/MfaD81+Efzw2JAZ647n5NP+5nrBubZktgFD8Z2t89I2PuyC
		//Encrypted string: tEEpbs9kHuh3RJ/GHXTEwGQe1MRVRGdqu/F6/WqDp6rTH3fx48YI8KT815u4KmXihFnDsPV1z0idVCiZqPkFY9xBqxouMamH
		string Msg;
		Msg = "하아정기야.This world is round, not flat, don't believe them!";
		Msg = "http://th-p.talk.kakao.co.kr/th/talkp/wke3o4tbAc/z5Zr5k02IhSrmU4NvTyxf1/kx19xk_110x110_c.jpg";
		Msg = "가나다라마바사!@#$%^&*()-=|";
		Msg = "0123456789";
		Msg = "";
		Msg = "http://th-p.talk.kakao.co.kr/th/talkp/wkfjowqYrv/dpGpFdOL1r2k99gQVspP20/4haroa_110x110_c.jpg";
		Msg = "farmgirl";
			
		string EncryptedString = EncryptString(Msg);
		string DecryptedString = DecryptString(EncryptedString);
#if UNITY_EDITOR
		Debug.Log("Message: " + Msg);
		Debug.Log("Encrypted string: " + EncryptedString);
		Debug.Log("Decrypted string: " + DecryptedString);
#endif
	}
		
		
	public static string EncryptString(string Message){
		if(Message == null || Message.Equals(""))return "";
		//else if(Message.Equals("farmgirl"))return Message;
		string Passphrase = strkey;
		byte[] Results;
			
		// Step 1. MD5 해쉬를 사용해서 암호화하고,	   
		// MD5 해쉬 생성기를 사용해서 결과는 128 비트 바이트 배열인데,          
		// 3DES 인코딩을 위한 올바른 길이가 됨.		
		System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
		MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();            
		byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));            
			
		// Step 2. TripleDESCryptoServiceProvider object 생성            
		TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
			
		// Step 3. Encoder 설정
		TDESAlgorithm.Key = TDESKey;            
		TDESAlgorithm.Mode = CipherMode.ECB;            
		TDESAlgorithm.Padding = PaddingMode.PKCS7;
			     
			
		try        
		{      
			// Step 4. 암호화할 문자열을 Byte[]로 변환          
			byte[] DataToEncrypt = UTF8.GetBytes(Message);    
					
			// Step 5. 실제로 문자열을 암호화      
			ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
			Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length); 
		}catch{
			//Debug.LogError ( e.ToString () );
#if UNITY_EDITOR
			Debug.LogWarning("SSUtil > EncryptString > error:" + Message);
#endif
			return "";
		}finally{
			// 중요한 3DES, Hashprovider의 속성을 해제       
			TDESAlgorithm.Clear();        
			HashProvider.Clear();
		}
			
		// Step 6. 암호화된 문자열을 Base64로 변환하여 리턴      
		return System.Convert.ToBase64String(Results);    
	}     
	    public static string DecryptString(string Message){  
		if(Message == null || Message.Equals(""))return "";
		//else if(Message.Equals("farmgirl"))return Message;
		string Passphrase = strkey;
		byte[] Results;
			
		System.Text.UTF8Encoding 	UTF8 			= new System.Text.UTF8Encoding();        
		MD5CryptoServiceProvider 	HashProvider 	= new MD5CryptoServiceProvider();       
		byte[] 						TDESKey 		= HashProvider.ComputeHash ( UTF8.GetBytes ( Passphrase ) );
			
		// Step 2. TripleDESCryptoServiceProvider object 생성       
		TripleDESCryptoServiceProvider 		TDESAlgorithm 		= new TripleDESCryptoServiceProvider();
			
		// Step 3. Decoder 설정
		TDESAlgorithm.Key 		= TDESKey;       
		TDESAlgorithm.Mode 		= CipherMode.ECB;       
		TDESAlgorithm.Padding 	= PaddingMode.PKCS7;       
			
			
			
		try{
			// Step 4. 인자로 받은 문자열을 Byte[]로 변환       
			byte[] DataToDecrypt = System.Convert.FromBase64String(Message);
			
			// Step 5. 실제 문자열 복호화
			ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
			Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
		}catch{
#if UNITY_EDITOR
			Debug.LogWarning("SSUtil > DecryptString > error:" + Message);
#endif
			return "";
		}finally{          
			// 중요한 3DES, Hashprovider의 속성을 해제
			TDESAlgorithm.Clear();                
			HashProvider.Clear();  
		}
		// Step 6. UTF-8 형태로 복호화된 문자열 리턴 
		return UTF8.GetString( Results );     
	}
		

	// read xml, txt file(Resources)
	public static string load(string _file){
		#if DEBUG_ON
			Debug.Log("SSUtil load _file:" + _file);
		#endif

		//1. file read and return
		TextAsset _textAsset = (TextAsset)Resources.Load(_file);
		return _textAsset.text;
	}
		
	//jar(Resources folder xx.bytes) image to out file
	public static string getResourcesBytesToOutFile(string _filename){
		string _imagePath = Application.persistentDataPath + "/" + _filename + ".png";
		if (!System.IO.File.Exists(_imagePath)){
			TextAsset _w = Resources.Load(_filename) as TextAsset;
			if( _w.bytes != null ){
				System.IO.FileStream _fs = new System.IO.FileStream(_imagePath, System.IO.FileMode.Create);
				_fs.Write(_w.bytes, 0, _w.bytes.Length);
				_fs.Close();
			}
		}
		return _imagePath;
	}
		
	//capture screen shot to out file.
	public static string captureScreenShotToOutFile(string _filename){
		_filename = _filename + ".png";
		string _imagePath = Application.persistentDataPath + "/" + _filename;
		ScreenCapture.CaptureScreenshot(_filename);
		return _imagePath;
	}
		
	/////////////////////////////////////////////////////
	//	NGUI GameObject Destroy.
	//	NGUI오브젝트를 직접 삭제거하면 오류가 발생함.(내부적으로 UnityEngine.Object.DestroyImmediate)
	// 	그래서 FixedUpdate, Update > 후에 파괴하기 (WaitForEndOfFrame)
	//
	//	사용법
	//			GameObject go;
	//			.....
	//			StartCoroutine(SSUtil.DestroyNGUI(go));
	/////////////////////////////////////////////////////
	public static IEnumerator DestroyNGUI(GameObject _go){
		yield return new WaitForEndOfFrame();
		UnityEngine.Object.Destroy(_go);
	}

	/// ////////////////////////////////////////////////////////////////////
	//_strName		: /tmp_1366364214754.png
	//_strURL		: http://images.earthcam.com/ec_metros/ourcams/tmp_1366364214754.png
	//_strPathFolder:
	//	Android		  /mnt/sdcard/Android/data/com.sangsangdigital.farmtycoongg/files
	//	PC			  C:/Documents and Settings/Administrator/Local Settings/Application Data/SangSangDigital/wwwwebhandler
	//_strPathFile	:
	//	Android		  /mnt/sdcard/Android/data/com.sangsangdigital.farmtycoongg/files/tmp_1366364214754.png
	//	PC			  C:/Documents and Settings/Administrator/Local Settings/Application Data/SangSangDigital/wwwwebhandler/tmp_1366364214754.png
	/// ////////////////////////////////////////////////////////////////////
	public static string getDataPath(string _strName){
		return Application.persistentDataPath + "/" + _strName;
	}

	public static string getDataPathProtocol(string _strName){
		#if UNITY_EDITOR
			_strName = "file://c://" + Application.persistentDataPath + "/" + _strName;
		#elif UNITY_ANDROID || UNITY_IPHONE
			_strName = "file://" + Application.persistentDataPath + "/" + _strName;
		#elif UNITY_WEBPLAYER
			_strName = _strURL;
		#else
			_strName = "file://c://" + Application.persistentDataPath + "/" + _strName;
		#endif


		return _strName;
	}


	// 패스워드 암호화.
	//public static string getGuestPassword(){
	//	string _password = "";
	//	/
	//	for(int i = 0; i < 8; i++){
	//		if(i%2 == 0){
	//			_password += Random.Range(0, 10);
	//		}else{
	//			_password += (char)(97 + Random.Range(0, 25));
	//		}
	//	}
	//	return _password;
	//}

	public static string setPassword(string _password){
		System.Text.UTF8Encoding _encoding = new System.Text.UTF8Encoding();
		int _len = _password.Length;
		int _lenMax = 20+1;
		int _lenMarge = _lenMax - _len;
	  	byte[] _arr = _encoding.GetBytes(_password);
		int _tmp, _loop, i;
		byte _sum = 0;
		bool _bSwitch = false;

		for(int k = 0; k < _lenMarge; ++k){
			//1. password reparsing.
			_arr = _encoding.GetBytes(_password);
			_sum = 0;

			//2. sum data.
			for(i = 0; i < _len; i++){
				_sum += _arr[i];
			}
			_loop = _sum % 10;
			if(_loop == 0){
				_loop = 4;
			//}else if(_loop == 5){
			//	_loop = 6;
			}

			//3. loop > jump
			for(i = 0; i < _len; i++){
				_loop = 10 - _loop;
				_tmp = (int)_arr[i];
				if(_tmp >= 48 && _tmp <= 57){
					_tmp += _loop;
					if(_tmp > 57)_tmp -= 10;
				}else if(_tmp >= 65 && _tmp <= 90){
					_tmp += _loop;
					if(_tmp > 90)_tmp -= 26;
				}else if(_tmp >= 97 && _tmp <= 122){
					_tmp += _loop;
					if(_tmp > 122)_tmp -= 26;
				}else{
					_tmp += _loop;
				}
				_arr[i] = (byte)_tmp;
			}
			_password = System.Text.Encoding.UTF8.GetString(_arr, 0, _arr.Length);

			//4. 추가문자열 만들기.
			if(_password.Length < _lenMax - 1){
				if(_bSwitch){
					_password = _password + _loop;
				}else{
					_password = _loop + _password;
				}
				_bSwitch = !_bSwitch;
			}
			//Debug.Log(_password);
		}



		return _password;
	}
		
	// 결제 암호화.
	public static string setCashEncode(string _strGameid, int _cash, int _goldball){
		string _strRtn = "";
		string _strCash = "";
		string _strGoldball = "";
		string _strSum = "";
		int _len, _loop;


		//1. loop.
		_loop = Random.Range(1, 10);
		_strRtn += _loop;
		#if DEBUG_ON
			Debug.Log("_loop:" + _loop);
		#endif

		//2. cash
		_cash = (_cash + 12345678);
		_strCash = "" + _cash;
		_len = _strCash.Length;
		#if DEBUG_ON
			Debug.Log("_strCashS:" + _strCash + "(" + (_cash - 12345678) + ")");
		#endif
		while(_len < 8){
			_strCash = "0" + _strCash;
			++_len;
		}
		_strRtn += _strCash;
		#if DEBUG_ON
			Debug.Log("_strCashE:" + _strCash);
		#endif

		//2-2. goldball
		_goldball = (_goldball + 87654321);
		_strGoldball = "" + _goldball;
		_len = _strGoldball.Length;
		#if DEBUG_ON
			Debug.Log("_strGoldballS:" + _strGoldball+  "(" + (_goldball - 87654321) + ")");
		#endif
		while(_len < 8){
			_strGoldball = "0" + _strGoldball;
			++_len;
		}
		_strRtn += _strGoldball;
		#if DEBUG_ON
			Debug.Log("_strGoldballE:" + _strGoldball);
		#endif

		//3. id.
		_strRtn += _strGameid;

		//4. current date.
		string _time = System.DateTime.Now.ToString("yyyyMMddhhmmss") + Random.Range(10, 99);
		_strRtn += _time;
		#if DEBUG_ON
			Debug.Log("_time:" + _time);
		#endif

		//5. summar
		#if DEBUG_ON
			Debug.Log("o:[" + _strRtn+"]");
		#endif

		System.Text.UTF8Encoding _encoding=new System.Text.UTF8Encoding();
		_len = _strRtn.Length;
	    byte[] _arr = _encoding.GetBytes(_strRtn);
		int _tmp;
		byte _sum = 0;
		for(int i = 1; i < _len; i++){
			//Debug.Log("_arr["+i+"]:" + _arr[i]);
			_tmp = (int)_arr[i];
			_sum += _arr[i];
			if(_tmp >= 48 && _tmp <= 57){
				_tmp += _loop;
				if(_tmp > 57)_tmp -= 10;
			}else if(_tmp >= 65 && _tmp <= 90){
				_tmp += _loop;
				if(_tmp > 90)_tmp -= 26;
			}else if(_tmp >= 97 && _tmp <= 122){
				_tmp += _loop;
				if(_tmp > 122)_tmp -= 26;
			}else{
				_tmp += _loop;
			}
			_arr[i] = (byte)_tmp;
		}

		_strSum = "" + _sum;
		_len = _strSum.Length;
		#if DEBUG_ON
			Debug.Log("_strSumS:" + _strSum);
		#endif
		while(_len < 3){
			_strSum = "0" + _strSum;
			++_len;
		}
		#if DEBUG_ON
			Debug.Log("_strSumE:" + _strSum);
		#endif

		_strRtn = System.Text.Encoding.UTF8.GetString(_arr, 0, _arr.Length);
		#if DEBUG_ON
			Debug.Log("c:[" + _strRtn+"]");
		#endif

		_strRtn = _strRtn + _strSum;

		#if DEBUG_ON
			Debug.Log("d:[" + _strRtn+"]");
		#endif

		//6. dummy 3byte
		_strRtn = Random.Range(100, 999) + _strRtn;


		return _strRtn;
	}

	////////////////////////////////////////////////////////////////////////////////////////////
	public static int getInt(string _param){

		if(_param == null){
			return -1;
		}else{
			int _rtn = -1;
			try{
				_rtn = System.Convert.ToInt32(_param);
			}catch(System.FormatException e){
				//#if DEBUG_ON
					Debug.LogError("#### SSUtil getInt error("+_param+"):" + e);
				//#endif
			}
			return _rtn;
		}
	}

	public static int getInt(string _param, int _startIdx, int _len){
		if(_param == null){
			return -1;
		}else if(_param.Length < _startIdx + _len){
			return -1;
		}else{
			int _rtn = -1;
			try{
				_rtn = System.Convert.ToInt32(_param.Substring(_startIdx, _len));
			}catch(System.FormatException e){
				//#if DEBUG_ON
					Debug.LogError("#### SSUtil getInt error("+_param+"):" + e);
				//#endif
			}
			return _rtn;
		}
	}

	public static long getLong(string _param){
		if(_param == null){
			return -1;
		}else{
			long _rtn = -1;
			try{
				_rtn = System.Convert.ToInt64(_param);
			}catch(System.FormatException e){
				//#if DEBUG_ON
					Debug.LogError("#### SSUtil getInt error("+_param+"):" + e);
				//#endif
			}
			return _rtn;
		}
	}

	public static string getString(byte[] _b){
		return enc.GetString(_b);
	}

	public static bool isURL(string _url){
			
		if ( _url == null ) {
			return false;
		}
			
		_url = _url.Trim();

		if(_url.IndexOf("http://") == 0)
			return true;
		else if(_url.IndexOf("https://") == 0)
			return true;
		else
			return false;
	}
		
	public static string getFilenameFromURL(string _url){
		string[] _tokens = _url.Split('/');
		return "/" + _tokens[_tokens.Length - 1];
	}
		
	//고유의  시리얼 키를 얻어오기.
	public static string getRandSerial(){
		string _str = getDateTimeTicks();
		char _rand, _rand2;
			
		for(int i = 0; i < 4; i++){
			_rand  = (char)('0' + Random.Range(0, 9));
			_rand2 = (char)('A' + Random.Range(0, 26));
			_str   = _str.Replace(_rand, _rand2);
		}
			
		return _str;
	}
		
	private static string getDateTimeTicks(){
		return System.DateTime.Now.Ticks.ToString();
	}
		
		
	//3. 시간차 구하기.
	// 어떤 시간이 넣으면 현재 시간을 지나갔는가?
	// true 	: 지정 시간지남.	> 일반모드.
	// false 	: 아직 시간남음. 	> x2모드.
	public static bool isPassedDate(string _strStartDate){
		System.DateTime _dtPoint = System.DateTime.Parse(_strStartDate);
		System.DateTime _dtNow = System.DateTime.Now;
		System.TimeSpan _sp = _dtNow - _dtPoint;

		return _sp.Milliseconds < 0 ? true : false;
	}

	//배틀기탐 정보.
	//public static string setEncode4(string _str){
	public static string setPhoneNumber(string _str){
		string _strRtn = "";
		string _strSum = "";
		int _len, _loop;

		//1. loop(1)
		_loop = Random.Range(1, 10);
		_strRtn += _loop;
		#if DEBUG_ON
			Debug.Log("1:[" + _strRtn+"]");
		#endif

		//2. size(3)
		_len = (_str.Length + "").Length;
		while(_len < 3){
			_strRtn += "0";
			++_len;
		}
		_strRtn += _str.Length;
		#if DEBUG_ON
			Debug.Log("2:[" + _strRtn+"]");
		#endif

		//3. data(x)
		_strRtn += _str;
		#if DEBUG_ON
			Debug.Log("3:[" + _strRtn+"]");
		#endif

		//4. empty random (1 + 3 + x + y = 20)
		_len = _strRtn.Length;
		while(_len <= 10){
			_strRtn += Random.Range(0, 10);
			++_len;
		}
		#if DEBUG_ON
			Debug.Log("4:[" + _strRtn+"]");
		#endif


		//4. current date.
		string _time = System.DateTime.Now.ToString("yyyyMMddhhmmss");
		_strRtn += _time;
		#if DEBUG_ON
			Debug.Log("_time:" + _time);
		#endif

		//5. data mix
		System.Text.UTF8Encoding _encoding=new System.Text.UTF8Encoding();
		_len = _strRtn.Length;
	    byte[] _arr = _encoding.GetBytes(_strRtn);
		int _tmp;
		byte _sum = 0;
		for(int i = 1; i < _len; i++){
			//Debug.Log("_arr["+i+"]:" + _arr[i]);
			_tmp = (int)_arr[i];
			_sum += _arr[i];
			if(_tmp >= 48 && _tmp <= 57){
				_tmp += _loop;
				if(_tmp > 57)_tmp -= 10;
			}else if(_tmp >= 65 && _tmp <= 90){
				_tmp += _loop;
				if(_tmp > 90)_tmp -= 26;
			}else if(_tmp >= 97 && _tmp <= 122){
				_tmp += _loop;
				if(_tmp > 122)_tmp -= 26;
			}else{
				_tmp += _loop;
			}
			_arr[i] = (byte)_tmp;
		}

		_strRtn = System.Text.Encoding.UTF8.GetString(_arr, 0, _arr.Length);
		#if DEBUG_ON
			Debug.Log("5:[" + _strRtn+"]");
		#endif

		//6. sum(3)
		_strSum = "" + _sum;
		_len = _strSum.Length;
		while(_len < 3){
			_strSum = "0" + _strSum;
			++_len;
		}
		#if DEBUG_ON
			Debug.Log("_strSum:" + _strSum);
		#endif

		_strRtn = _strRtn + _strSum;
		/**/

		return _strRtn;
	}
}
