//#define DEBUG_ON
using UnityEngine;
using System.Collections;
using System.Xml;

public class SSParser{
	//private string filePath = "txt/help";
	//private string filePath = "txt/iteminfo";
	//private string fileName = "";
	//private TextAsset textAsset = null;
	private XmlDocument xmlDoc = null;
	private XmlNodeList xmlNode = null;
	private int cursor;
	//private XmlDocument[] xmlDocHelp = new XmlDocument[2];
	//private XmlNodeList[] xmlNodeHelp = new XmlNodeList[2];
	//private int[] cursorHelp = new int[2];


	/// <summary>
	/// 파싱할 XML 데이타(_str)와 분리할 파라미터(_sep)를 넣어주면 안에서 분리가 된다.
	/// </summary>
	public void parsing(string _str, string _sep){
		#if DEBUG_ON
			Debug.Log("SSParser parsing _str,  _sep:" + _sep);
		#endif
		release();

		xmlDoc = new XmlDocument() ;

		//Debug.Log ( _str );

		xmlDoc.LoadXml(_str);


		parsing(_sep);
	}

	public void parsing(string _sep){
		try{
			xmlNode = xmlDoc.GetElementsByTagName(_sep);
			cursor = -1;
		}
		catch{
			Debug.LogError(_sep);
		}
		#if DEBUG_ON
			Debug.Log("SSParser parsingSep > " + _sep + ":" + xmlNode.Count);
		#endif
	}

	public void release(){
		#if DEBUG_ON
			Debug.Log("SSParser release");
		#endif
		xmlDoc = null;
		xmlNode = null;
	}

	public bool next(){
		cursor++;

		#if DEBUG_ON
			Debug.Log("SSParser next cursor:" + cursor + ", xmlNode.Count:" + xmlNode.Count);
		#endif
//		try
//		{
		if(cursor < xmlNode.Count )
			return true;
		else
			return false;
//		}
//		catch
//		{
//			Debug.LogError( cursor + " " + xmlNode.ToString() );
//			return false;
//		}
	}

	public int getCount(){
		return xmlNode.Count;
	}

	public string getString(string _param){
		int _count2 = xmlNode[cursor].ChildNodes.Count;
		for( int j = 0 ; j < _count2; j++ ){
			if(_param.Equals(xmlNode[cursor].ChildNodes[j].Name)){
				return xmlNode[cursor].ChildNodes[j].InnerText;
			}
		}
		return null;
	}

	public int getInt(string _param){
		#if DEBUG_ON
			Debug.Log("SSParser getInt < getString("+_param+"):" + getString(_param));
		#endif
		_param = getString(_param);
		if(_param == null){
			return -1;
		}else{
			int _rtn = -1;
			try{
				_rtn = System.Convert.ToInt32(_param);
			}catch(System.FormatException e){
				//#if DEBUG_ONtr
#if UNITY_EDITOR
				Debug.LogError("#### SSParser getInt error("+_param+"):" + e);
#endif
				//#endif
			}
			return _rtn;
		}
	}
		
	//@@@@ int 범위초과 값에 대한 검사.
	public long getInt64(string _param){
		#if DEBUG_ON
			Debug.Log("SSParser getInt < getString("+_param+"):" + getString(_param));
		#endif
		_param = getString(_param);
		if(_param == null){
			return -1;
		}else{
			long _rtn = -1;
			try{
				_rtn = System.Convert.ToInt64(_param);
			}catch(System.FormatException e){
				//#if DEBUG_ON
					Debug.LogError("#### SSParser getInt64 error("+_param+"):" + e);
				//#endif
			}
			return _rtn;
		}
	}

	//12345.6789 > 12345.6789
	//Convert.ToDouble
	public float getFloat(string _param){
		_param = getString(_param);
		if(_param == null){
			return -1;
		}else{
			return (float)System.Convert.ToDouble( _param);
		}
	}

	//////////////////////////////////////
	public string getStringRow(int _cursor, string _param){
		cursor = _cursor - 1;
		if(next()){
			int _count2 = xmlNode[cursor].ChildNodes.Count;
			for( int j = 0 ; j < _count2; j++ ){
				if(_param.Equals(xmlNode[cursor].ChildNodes[j].Name)){
					return xmlNode[cursor].ChildNodes[j].InnerText;
				}
			}
		}
		return null;
	}

	public string getStringNext(string _param){
		if(!next()){
			cursor = 0;
		}

		int _count2 = xmlNode[cursor].ChildNodes.Count;
		for( int j = 0 ; j < _count2; j++ ){
			if(_param.Equals(xmlNode[cursor].ChildNodes[j].Name)){
				return xmlNode[cursor].ChildNodes[j].InnerText;
			}
		}
		return null;
	}
}
