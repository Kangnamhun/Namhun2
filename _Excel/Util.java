
import java.util.Random;

class Util{
	///////////////////////////////////////////////////
	// 자바의 형태으로 저장 할때 사용 => byte[4] 0 1 2 3
	///////////////////////////////////////////////////
	public static byte[] setJInt(int data){
		byte ret[] = new byte[4];
		ret[0] = (byte)(data >> 24);
		ret[1] = (byte)(data >> 16);
		ret[2] = (byte)(data >> 8);
		ret[3] = (byte)(data >> 0);
		return ret;
	}

    public static void setJInt(byte data[], int idx, int value){
    	data[idx    ] = (byte)(value >> 24 & 0xff);
    	data[idx + 1] = (byte)(value >> 16 & 0xff);
    	data[idx + 2] = (byte)(value >> 8  & 0xff);
        data[idx + 3] = (byte)(value  	   & 0xff);
    }

	//////////////////////////////////////////////////////////////
	//		자바형태에서 데이타 읽기
	//		PLTE Change CRC Check
	/////////////////////////////////////////////////////////////
	// 1. 캐스팅 => 쉬프트 => 필터 => 종합 => 빠름
	// 2. 캐스팅 => 필터 => 쉬프트 => 종합 => 약간느리고 상위 어플로우 발생
    public static int getJInt(byte data[], int idx){
		return (   (int)data[idx] 	<< 24 & 0xFF000000)
				| ((int)data[idx+1] << 16 & 0x00FF0000)
				| ((int)data[idx+2] << 8  & 0x0000FF00)
				| ((int)data[idx+3]       & 0x000000FF);
    }

    //데이타가 순서대로 RGB값이 들어가 있을 경우 추출
	public static int getRGB(byte[] data, int idx){
		return (  (((int)data[idx + 0] << 16) & 0x00FF0000)
				| (((int)data[idx + 1] << 8 ) & 0x0000FF00)
				| (((int)data[idx + 2]      ) & 0x000000FF));
	}

	////////////////////////////////////////////////////////////////////
	//			C형 자료구조로 저장하기
	///////////////////////////////////////////////////////////////////
	public static int setCInt(byte[] buf, int idx, int data){
		buf[idx + 3] = (byte)(data >> 24);
		buf[idx + 2] = (byte)(data >> 16);
		buf[idx + 1] = (byte)(data >> 8);
		buf[idx + 0] = (byte)(data >> 0);
		return idx + 4;
	}
	public static int setCShort(byte[] buf, int idx, int data){
		buf[idx + 1] = (byte)(data >> 8);
		buf[idx + 0] = (byte)(data >> 0);
		return idx + 2;
	}

	//////////////////////////////////////////////////////////////////////
	//			C => Java
	/////////////////////////////////////////////////////////////
	public static int getCByte(byte[] data, int idx){
		//return (((int)data[idx + 0]) & 0x000000FF);
		return (int)(data[idx]     & 0x000000FF);
	}

	public static short getCShort(byte[] data, int idx){
		return (short)((((int)data[idx + 1] << 8 ) & 0x0000FF00)
					 | (((int)data[idx + 0]      ) & 0x000000FF));
	}

	//직전 레퍼런스 연산
	public static int getCInt(byte[] data, int idx){
		// 1. 캐스팅 => 쉬프트 => 필터 => 종합 => 빠름
		// 2. 캐스팅 => 필터 => 쉬프트 => 종합 => 약간느리고 상위 어플로우 발생
		return (  (((int)data[idx + 3] << 24) & 0xFF000000)
				| (((int)data[idx + 2] << 16) & 0x00FF0000)
				| (((int)data[idx + 1] << 8 ) & 0x0000FF00)
				| (((int)data[idx + 0]      ) & 0x000000FF));
	}

    public static String getCString(byte[] source, int index, int len){
    	int start = index, end = index + len;
    	//System.out.println(source[end - 1]);
    	if(len == 0){
    		//System.out.println("없내");
    		return "";
    	}else if(source[end - 1] == 0){
    		index = end - 1;
    		//System.out.println("1 source["+index+"]:"+source[index]);
    	}else{
	    	for(; index < end; ++index){
	    		//if(Constant.DEBUG)
	    		//	System.out.println("2 source["+index+"]:"+source[index]);
	    		if(source[index] == 0)
	    			break;
	    	}
	    }
    	///if(Constant.DEBUG)
    		///System.out.println(start+":"+(index - start)+":"+(new String(source, start, (index - start))));
    	//return (new String(source, start, (index - start))).trim();
    	return (new String(source, start, (index - start)));
    }

    public static int setCString(byte _src[], byte[] _tar, int _index){
    	//원문자 + 0[끝문자열] => 원문자0 이와 같이 변형한다.
    	byte _src2[] = new byte[_src.length + 1];
    	System.arraycopy(_src, 0, _src2, 0, _src.length);

    	System.arraycopy(_src2, 0, _tar, _index, _src2.length);
    	return _index + _src2.length;
    }

	/*
	///////////////////////////////////////////////////
	//		300000 => 300,000
	///////////////////////////////////////////////////
	public static String displayMoney(String str){
		String tmp = "";
		int size = str.length();
		do{
			tmp = str.substring(size-3,size)+","+tmp;
			str = str.substring(0, size-3);
		}while(size > 3);
		return (str+","+tmp.substring(0, tmp.length()-1));
	}
	/**/

	////////////////////////////////////////////////////
	//		//(0,4) => 0,1,2,3 까지 나온다.
	//(주의)	연속에서 바로 호출하면 같은 번호가 나올 가능성이 높다.
	////////////////////////////////////////////////////
	public static int random(int min, int max){
		if(min == max){
			//0 , 0 같으면 에러나니까 같지 않게 만든다.
			max+=1;
		}else if(min > max){
			//작은 값이 더 크면 스왑한다.
			// 0 , -1
			int tmp = max;
			max = min;
			min = tmp;
		}
		Random random = new Random();
		return Math.abs(random.nextInt())%(max - min) + min;
	}

	//문자열 특수하게 분리하기
	//replace("hi\n\n", "\n", ""+'\n', 2);
	public static String replace(String _src, String _matchStr, String _toChange, int _size){
		if(Constant.DEBUG)
			System.out.println("_src:" + _src
								+"\n _matchStr:" +_matchStr
								+"\n _toChange:" +_toChange
								+"\n _size:" +_size);
		String tmpStr = "", tmpStr2 = "", tmpStr3 = "";
		int index, index2, _len = _matchStr.length();
		while(_src.length() != 0){
			index = _src.indexOf(_matchStr);
			//System.out.println("index:"+index);
			if(index < 0){
				tmpStr += _src;
				_src = "";
			}else{
				//tmpStr2 = _src.substring(0, index);
				//while(tmpStr2.length() != 0){
				//	if(tmpStr2.length() > _size){
				//		tmpStr3 =tmpStr3 + tmpStr2.substring(0,_size)+_toChange;
				//		tmpStr2 = tmpStr2.substring(_size);
				//	}else{
				//		tmpStr3 +=tmpStr2;//_size = 4
				//		tmpStr2 = "";
				//	}
				//}
				tmpStr += _src.substring(0, index);
				_src = _src.substring(index + _matchStr.length());
				tmpStr += _toChange;
			}
		}
		return tmpStr;
	}

	//===================================================================
	public static String makeQuery(String _str){
		return "'" + _str + "'";
	}

	//===================================================================
	//변환용
	public static boolean isNumber(String _str){
		boolean _bFind = false;
		int _si = _str.indexOf("(");
		int _ei = _str.indexOf(")");
		if(_si >= 0 && _ei >= 0 && _si < _ei){
			_bFind = true;
		}

		char _word = _str.charAt(0);
		if(_word >= '0' && _word <= '9' || _word == '-'){
			_bFind = true;
		}

		return _bFind;
	}

	public static int parseInt(String _str, String _comment){
		int _ri = -1;
		int _startIdx = -1;
		int _endIdx = -1;
		//System.out.println(_str);

		if(_str.indexOf("(") >= 0 && _str.indexOf(")") >= 0){
			//()에 의해서 묶여진것
			//(-1), (0), (1), ....
			if(_str.indexOf("(+") >= 0)
				_startIdx = _str.indexOf("(+")+1;
			else
				_startIdx = _str.indexOf("(");
			_endIdx = _str.indexOf(")");
			if(_startIdx >= 0){
				//System.out.println("원판:" + _str);
				_str = _str.substring(_startIdx + 1, _endIdx);
				//System.out.println("변환:" + _str);
			}
		}else if(_str.indexOf("Q") >= 0){
			//Q마크가 있는것
			//Q11, Q22, Q13, ...
			_startIdx = _str.indexOf("Q");
			_endIdx = _str.length();
			if(_startIdx >= 0){
				//System.out.println("원판:" + _str);
				_str = _str.substring(_startIdx + 1, _endIdx);
				//System.out.println("변환:" + _str);
			}
		}else{
			//숫자형이 들어옴 => 패스
			//... -1, 0, +1, ....
		}

		try{
			_ri = Integer.parseInt(_str);
		}catch(Exception e){
			System.out.println("#### "+_comment+" => 문자 -> 정수로변환안됨 [" + _str + "]");
		}
		return _ri;
	}

	public static String getStringInt(String _str){
		String _strOriginal = _str;
		int _startIdx = -1;
		int _endIdx = -1;
		//System.out.println(_str);

		if(_str.indexOf("(") >= 0 && _str.indexOf(")") >= 0){
			//()에 의해서 묶여진것
			//(-1), (0), (1), ....
			if(_str.indexOf("(+") >= 0)
				_startIdx = _str.indexOf("(+")+1;
			else
				_startIdx = _str.indexOf("(");
			_endIdx = _str.indexOf(")");
			if(_startIdx >= 0){
				//System.out.println("원판:" + _str);
				_str = _str.substring(_startIdx + 1, _endIdx);
				//System.out.println("변환:" + _str);
			}
		//}else if(_str.indexOf("Q") >= 0){
		//	//Q마크가 있는것
		//	//Q11, Q22, Q13, ...
		//	_startIdx = _str.indexOf("Q");
		//	_endIdx = _str.length();
		//	if(_startIdx >= 0){
		//		//System.out.println("원판:" + _str);
		//		_str = _str.substring(_startIdx + 1, _endIdx);
		//		//System.out.println("변환:" + _str);
		//	}
		}else{
			//숫자형이 들어옴 => 패스
			//... -1, 0, +1, ....
		}

		try{
			int _ri = Integer.parseInt(_str);
		}catch(Exception e){
			_str = _strOriginal;
		}

		return _str;
	}

	public static String getBraceString(String _str){
		int _startIdx = -1;
		int _endIdx = -1;

		if(_str.indexOf("(") >= 0 && _str.indexOf(")") >= 0){
			//()에 의해서 묶여진것
			//(-1), (0), (1), ....
			if(_str.indexOf("(+") >= 0)
				_startIdx = _str.indexOf("(+")+1;
			else
				_startIdx = _str.indexOf("(");
			_endIdx = _str.indexOf(")");
			if(_startIdx >= 0){
				//System.out.println("원판:" + _str);
				_str = _str.substring(_startIdx + 1, _endIdx);
				//System.out.println("변환:" + _str);
			}
		}else{
			//숫자형이 들어옴 => 패스
			//... -1, 0, +1, ....
		}
		return _str;
	}


	/*
	public static int parseInequality(String _str){
		int _ri = 3;
		if(_str.equals("==")){
			//System.out.println(_str + " : ==");
			_ri = 3;
		}else if(_str.equals(">")){
			//System.out.println(_str + " : >");
			_ri = 1;
		}else if(_str.equals(">=")){
			//System.out.println(_str + " : >=");
			_ri = 2;
		}else if(_str.equals("<=")){
			//System.out.println(_str + " : <=");
			_ri = 4;
		}else if(_str.equals("<")){
			//System.out.println(_str + " : <");
			_ri = 5;
		}else if(_str.equals("!=")){
			//System.out.println(_str + " : !=");
			_ri = 6;
		}
		return _ri;
	}

	public static String parseInequality2(byte _kind){
		String _str = "";
		switch(_kind){
			case 3:
				_str = "==(3)";
				break;
			case 1:
				_str = ">(1)";
				break;
			case 2:
				_str = ">=(2)";
				break;
			case 4:
				_str = "<=(4)";
				break;
			case 5:
				_str = "<(5)";
				break;
			case 6:
				_str = "!=(6)";
				break;
		}
		return _str;
	}
	/**/

	//출력용
	public static void debug(byte str){
		debug(""+str);
	}
	public static void debug(short str){
		debug(""+str);
	}
	public static void debug(int str){
		debug(""+str);
	}
	public static void debug(String str){
		System.out.println(str);
	}
	public void printHex(byte data){
		System.out.print(toHexa(data));
	}
	public void printHex2(byte data){
		System.out.print(toHexa(data)+" ");
	}
	public String byteToHexa(byte data){
		StringBuffer buf = new StringBuffer(Integer.toHexString((int)data & 0x000000FF));
		int len = buf.length();
		for(int i = 0 ; i < 2 - len ; i++)
			buf.insert(0,'0');
		return "0x"+buf.toString();
	}
	public String shortToHexa(short data){
		StringBuffer buf = new StringBuffer(Integer.toHexString((int)data & 0x0000FFFF));
		int len = buf.length();
		for(int i = 0 ; i < 4 - len ; i++)
			buf.insert(0,'0');
		return "0x"+buf.toString();
	}
	public String intToHexa(int data){
		StringBuffer buf = new StringBuffer(Integer.toHexString(data));
		int len = buf.length();
		for(int i = 0 ; i < 8 - len ; i++)
			buf.insert(0,'0');
		return "0x"+buf.toString();
	}

	public String toHexa(byte data){
		StringBuffer buf = new StringBuffer(Integer.toHexString((int)data & 0x000000FF));
		int len = buf.length();
		for(int i = 0 ; i < 2 - len ; i++)
			buf.insert(0,'0');
		//return "0x"+buf.toString();
		return buf.toString();
	}
	public char toAscii(byte data){
		return (char)data;
	}
}
