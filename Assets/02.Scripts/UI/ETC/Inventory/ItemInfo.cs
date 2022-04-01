using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ItemInfo : MonoBehaviour
{
    //아이콘이나 UI에 표시할 이미지 묶음이 들어있다
    //사용법 : atlas에 이름(string)을 넣어주면 해당 sprite가 리턴된다
    public SpriteAtlas attlas;
    #region sigletone
    public static ItemInfo ins;
    private void Awake()
    {
        ins = this;
        ReadAndParse();
        //아이템 데이터는 다른 어떤것보다 먼저 실행이 되어야 하므로 Awake에서 실행
    }
    #endregion
    //공통 데이터가 들어있는부분
    public Dictionary<int, ItemInfoBase> dic_Base = new Dictionary<int, ItemInfoBase>();

    //장비템의 공유 정보가 들어있는 부분 + 공통
    public Dictionary<int, ItemInfoWearPart> dic_WearPart = new Dictionary<int, ItemInfoWearPart>();

    //소비템의 공유 정보가 들어있는 부분 + 공통
    public Dictionary<int, ItemInfoUsepart> dic_UsePart = new Dictionary<int, ItemInfoUsepart>();

    //기타템의 공유 정보가 들어있는 부분 + 공통
    public Dictionary<int, ItemInfoETCpart> dic_ETCPart = new Dictionary<int, ItemInfoETCpart>();

    
    public Dictionary<int, ItemInfoCoinpart> dic_CoinPart = new Dictionary<int, ItemInfoCoinpart>();


    SSParser parser = new SSParser(); //XML(string)데이터를 분석해주는 클래스
    string strItemInfo;


    public void ReadAndParse()
    {
        //XML file데이터를 읽어서 string 변수에 넣어줌
        strItemInfo = SSUtil.load("txt/iteminfo"); //txt폴더에 iteminfo를 읽는다

        //string 변수에 있는 데이터중에 해당 lable(이름)으로 된것을 분석해서 class로 만든후 dic에 등록한다
        ParseWearPart(strItemInfo, "wearpart");     //ParseWearPart 에서 wearpart 들만 뽑아낸다(장비 아이템)
        ParseUsePart(strItemInfo, "usepart");      //ParseWearPart 에서 usepart 들만 뽑아낸다(소비 아이템)
        ParseETCPart(strItemInfo, "etcpart");      //ParseWearPart 에서 etcpart 들만 뽑아낸다(기타 아이템)
        ParseCoinPart(strItemInfo, "coinpart");     
    }

    #region common data (공통 데이터)
    public string GetItemInfoIcon(int _itemcode)
    {
        //itemcode 로 전체 dic를 검색해서 icon(string)이름을 찾아오기
        string _rtn = null;

        //itemcode로 dic에 있는가
        if (dic_Base.ContainsKey(_itemcode))
        {
            //있으면 해당아이콘 이름을 _rtn에 넣어주기
            _rtn = dic_Base[_itemcode].icon;
        }
        return _rtn;
    }

    public Sprite GetItemInfoSpriteIcon(int _itemcode) //아이템 코드로 불러오는것
    {
        //itemcode를 가지고 icon이름을 찾은후 그것을 atlas에서 이름(string)으로 검색해서 sprite를 찾아온다
        return attlas.GetSprite(GetItemInfoIcon(_itemcode));
    }

    public Sprite GetSprite(string _itemName) //아이템 이름으로 불러오는것
    {
        return attlas.GetSprite(_itemName);
    }

    public ItemInfoBase GetItemInfoBase(int _itemcode)
    {
        //itemcode로 검색해서 base부분에 클래스 데이터만 찾을때
        //base : itemcode.subcategory ~ description까지 들어있는 클래스 데이터
        ItemInfoBase _rtn = null;

        //itemcode로 dic에 있는가
        if (dic_Base.ContainsKey(_itemcode))
        {
            _rtn = dic_Base[_itemcode];
        }
        return _rtn;
    }
    #endregion

    #region wearpart
    //itemcode를 넣어주면 wearpart에있는 dic에서 해당 아이템을 검색해준다.
    //아이템이 있다면 해당 아이템 class를 리턴
    //아이템이 없다면 null을 리턴
    public ItemInfoWearPart GetItemInfoWearPart(int _itemcode)
    {
        ItemInfoWearPart _rtn = null;
        if (dic_WearPart.ContainsKey(_itemcode))
        {
           _rtn =  dic_WearPart[_itemcode];
        }
        return _rtn;
    }

    void ParseWearPart(string _src, string _label)
    {
        //XML에 있는 WearPart파트라는 이름으로 되어있는것들을 전부 분석한다.
        parser.parsing(_src, _label);

        ItemInfoWearPart _data;
        int _itemcode;
        while (parser.next())//parser.next를 만나면 
        {
            //현재 커서의 위치에 있는 아이템 정보를 분석 할수 있도록 커서(포인트)가 위치한다.
            //첫번째 담을 클래스를 하나 만들어둔다.
            _data = new ItemInfoWearPart();

            //아이템들은 모두다 itemcode로 관리되며 서버에 itemcode로 저장되고 유지되므로 이것이 키가된다.
            //위에 생성된 클래스를 관리하는 Dictioary에서는 itemcode로 관리되고 검색된다.
            _itemcode = parser.getInt("itemcode");

            //아이템 코드로 검색해서 있으면 이미 파싱했기때문에 2번째는 등록안함
            //아이템 코드로 검색해서 없으면 파싱된 데이터를 클래스에 넣어서 dictorary로 등록해준다.
            if (!dic_Base.ContainsKey(_itemcode))
            {
                //XML에 해당이름으로 검색을 해서 해당클래스 변수에 세팅해서 넣어준다
                _data.itemcode      = _itemcode;                    //itemcode 아이템 코드는 중복되지 않는다, 고유 키 역할을 한다
                _data.category      = parser.getInt("category");    //메인 카테고리로 장비템,소모품,기타템을 분류하는 역할을한다, 인벤토리 위치를 판단한다
                _data.subcategory   = parser.getInt("subcategory"); //장비템 중에서도 무기,방어구 처럼 서브 종류를 말한다, 무기를 장착할때 서브카테고리가 중복되면 해당카테고리는 벗어난다
                _data.equpslot      = parser.getInt("equpslot");    //장비 슬롯 위치
                _data.itemname      = parser.getString("itemname"); //아이템 이름을 표기
                _data.activate      = parser.getInt("activate");    //아이템 활성화 여부
                //_data.toplist       = parser.getInt("toplist");     
                _data.grade         = parser.getInt("grade");       //아이템의 등급(일반,레어,에픽)
                //_data.discount      = parser.getInt("discount");    아이템의 할인율
                _data.icon          = parser.getString("icon");     //아이템 아이콘을 표기한다
                _data.playerlv      = parser.getInt("playerlv");    //아이템의 레벨제한
                //_data.multistate    = parser.getInt("multistate");  세트아이템
                _data.gamecost      = parser.getInt("gamecost");    //돈, 사냥을하거나 물건을 팔거나 퀘스트를깨면 받는 돈
                //_data.cashcost      = parser.getInt("cashcost");  캐쉬
                _data.buyamount     = parser.getInt("buyamount");   //아이템 갯수,수량 
                _data.sellcost      = parser.getInt("sellcost");    //아이템 판매할때 금액
                _data.description   = parser.getString("description");  

                _data.plusatt   = parser.getFloat("plusatt");           // 장비 공격력, 플러스 공격력
                _data.plusdef   = parser.getFloat("plusdef");           // 장비 방어력, 플러스 방어력
                _data.plushp    = parser.getFloat("plushp");             // 장비 hp, 플러스 hp
                _data.plusmp    = parser.getFloat("plusmp");             // 장비 mp, 플러스 mp
                _data.skin      = parser.getString("skin");              //장비 키는거
                _data.skin2     = parser.getString("skin2");


                //만들어진 한개의 장비 데이터 클래스를 관리용 Dic에 넣어준다
                //이 관리용 dic는 데이터를 검색해서 사용한다
                //아이템 코드로 검색해서 나온 클래스(아이템 데이터)는 해당 itemcode에 모든 데이터가 들어있어서 게임의 아이템들을 표시한다\
                // 예) 아이템 방어력 출력할때  itemcode -> dic -> class.Plusdef가 장비 방어력이다
                // 예) 아이콘 출력할때 itemcode -> dic -> class.icon -> 아이콘 스프라이트 이름이 나옴, 이것을 atlas 에 넣어으면 바로 아이콘sprite가 나옴.
                dic_Base.Add(_itemcode, _data);
                dic_WearPart.Add(_itemcode, _data);
            }
        }
    }
    #endregion


    #region usepart
    //itemcode를 넣어주면 Usepart에있는 dic에서 해당 아이템을 검색해준다.
    //아이템이 있다면 해당 아이템 class를 리턴
    //아이템이 없다면 null을 리턴,  
    public ItemInfoUsepart GetItemInfoUsepart(int _itemcode)
    {
        ItemInfoUsepart _rtn = null;

        //dic_wearpart 안에 itemcode가 존재(포함)하는가?
        if (dic_UsePart.ContainsKey(_itemcode))
        {
            //존재하면 dic은 배열처럼 사용하는데 배열안에는 키값을 넣어주면 해당 데이터가 검색되어서 나옴
            //_rtn 는 실제 클래스 데이터가 들어있음
            _rtn = dic_UsePart[_itemcode];
        }
        return _rtn;
    }

    //XML에서 해당 이름으로 된것들을 파싱(분석)해서 dic에 넣어주는 부분.
    void ParseUsePart(string _src, string _label)
    {
        //파트 이름으로 Parsing
        parser.parsing(_src, _label);

        ItemInfoUsepart _data;
        int _itemcode;
        while (parser.next())//parser.next를 만나면 
        {
            //현재 커서의 위치에 있는 아이템 정보를 분석 할수 있도록 커서(포인트)가 위치한다.
            //첫번째 담을 클래스를 하나 만들어둔다.
            _data = new ItemInfoUsepart();

            //아이템들은 모두다 itemcode로 관리되며 서버에 itemcode로 저장되고 유지되므로 이것이 키가된다.
            //위에 생성된 클래스를 관리하는 Dictioary에서는 itemcode로 관리되고 검색된다.
            _itemcode = parser.getInt("itemcode");

            if (!dic_Base.ContainsKey(_itemcode))
            {
                //XML에 해당이름으로 검색을 해서 해당클래스 변수에 세팅해서 넣어준다
                _data.itemcode = _itemcode;                    //itemcode 아이템 코드는 중복되지 않는다, 고유 키 역할을 한다
                _data.category = parser.getInt("category");    //메인 카테고리로 장비템,소모품,기타템을 분류하는 역할을한다, 인벤토리 위치를 판단한다
                _data.subcategory = parser.getInt("subcategory"); //장비템 중에서도 무기,방어구 처럼 서브 종류를 말한다, 무기를 장착할때 서브카테고리가 중복되면 해당카테고리는 벗어난다
                _data.equpslot = parser.getInt("equpslot");    //장비 슬롯 위치
                _data.itemname = parser.getString("itemname"); //아이템 이름을 표기
                _data.activate = parser.getInt("activate");    //아이템 활성화 여부
                //_data.toplist       = parser.getInt("toplist");     
                _data.grade = parser.getInt("grade");       //아이템의 등급(일반,레어,에픽)
                //_data.discount      = parser.getInt("discount");    아이템의 할인율
                _data.icon = parser.getString("icon");     //아이템 아이콘을 표기한다
                _data.playerlv = parser.getInt("playerlv");    //아이템의 레벨제한
                //_data.multistate    = parser.getInt("multistate");  세트아이템
                _data.gamecost = parser.getInt("gamecost");    //돈, 사냥을하거나 물건을 팔거나 퀘스트를깨면 받는 돈
                //_data.cashcost      = parser.getInt("cashcost");  캐쉬
                _data.buyamount = parser.getInt("buyamount");   //아이템 갯수,수량 
                _data.sellcost = parser.getInt("sellcost");    //아이템 판매할때 금액
                _data.description = parser.getString("description");

                _data.hp = parser.getInt("hp"); //소비아이템 hp 회복
                _data.mp = parser.getInt("mp"); //소비아이템 mp회복
                

                //dic에 Add
                dic_Base.Add(_itemcode, _data);
                dic_UsePart.Add(_itemcode, _data);
            }
        }
    }
    #endregion

    #region ETCpart
    //itemcode를 넣어주면 Usepart에있는 dic에서 해당 아이템을 검색해준다.
    //아이템이 있다면 해당 아이템 class를 리턴
    //아이템이 없다면 null을 리턴, 
    public ItemInfoETCpart GetItemInfoETCPart(int _itemcode)
    {
        ItemInfoETCpart _rtn = null;

        //dic_wearpart 안에 itemcode가 존재(포함)하는가?
        if (dic_ETCPart.ContainsKey(_itemcode))
        {
            //존재하면 dic은 배열처럼 사용하는데 배열안에는 키값을 넣어주면 해당 데이터가 검색되어서 나옴
            //_rtn 는 실제 클래스 데이터가 들어있음
            _rtn = dic_ETCPart[_itemcode];
        }
        return _rtn;
    }

    //XML에서 해당 이름으로 된것들을 파싱(분석)해서 dic에 넣어주는 부분.
    void ParseETCPart(string _src, string _label)
    {
        //파트 이름으로 Parsing
        parser.parsing(_src, _label);

        ItemInfoETCpart _data;
        int _itemcode;
        while (parser.next())//parser.next를 만나면 
        {
            //현재 커서의 위치에 있는 아이템 정보를 분석 할수 있도록 커서(포인트)가 위치한다.
            //첫번째 담을 클래스를 하나 만들어둔다.
            _data = new ItemInfoETCpart();

            //아이템들은 모두다 itemcode로 관리되며 서버에 itemcode로 저장되고 유지되므로 이것이 키가된다.
            //위에 생성된 클래스를 관리하는 Dictioary에서는 itemcode로 관리되고 검색된다.
            _itemcode = parser.getInt("itemcode");

            if (!dic_Base.ContainsKey(_itemcode))
            {
                //XML에 해당이름으로 검색을 해서 해당클래스 변수에 세팅해서 넣어준다
                _data.itemcode = _itemcode;                    //itemcode 아이템 코드는 중복되지 않는다, 고유 키 역할을 한다
                _data.category = parser.getInt("category");    //메인 카테고리로 장비템,소모품,기타템을 분류하는 역할을한다, 인벤토리 위치를 판단한다
                _data.subcategory = parser.getInt("subcategory"); //장비템 중에서도 무기,방어구 처럼 서브 종류를 말한다, 무기를 장착할때 서브카테고리가 중복되면 해당카테고리는 벗어난다
                _data.equpslot = parser.getInt("equpslot");    //장비 슬롯 위치
                _data.itemname = parser.getString("itemname"); //아이템 이름을 표기
                _data.activate = parser.getInt("activate");    //아이템 활성화 여부
                //_data.toplist       = parser.getInt("toplist");     
                _data.grade = parser.getInt("grade");       //아이템의 등급(일반,레어,에픽)
                //_data.discount      = parser.getInt("discount");    아이템의 할인율
                _data.icon = parser.getString("icon");     //아이템 아이콘을 표기한다
                _data.playerlv = parser.getInt("playerlv");    //아이템의 레벨제한
                //_data.multistate    = parser.getInt("multistate");  세트아이템
                _data.gamecost = parser.getInt("gamecost");    //돈, 사냥을하거나 물건을 팔거나 퀘스트를깨면 받는 돈
                //_data.cashcost      = parser.getInt("cashcost");  캐쉬
                _data.buyamount = parser.getInt("buyamount");   //아이템 갯수,수량 
                _data.sellcost = parser.getInt("sellcost");    //아이템 판매할때 금액
                _data.description = parser.getString("description");

                /*_data.plusatt = parser.getInt("plusatt"); 기타 아이템은 필요없음
                _data.plusdef = parser.getInt("plusdef");
                _data.plushp = parser.getInt("plushp");*/

                //dic에 Add
                dic_Base.Add(_itemcode, _data);
                dic_ETCPart.Add(_itemcode, _data);
            }
        }
    }
    #endregion

    #region coinpart

    public ItemInfoCoinpart GetItemInfoCoinPart(int _itemcode)
    {
        ItemInfoCoinpart _rtn = null;
        if (dic_CoinPart.ContainsKey(_itemcode))
        {
            _rtn = dic_CoinPart[_itemcode];
        }
        return _rtn;
    }

    void ParseCoinPart(string _src, string _label)
    {
        parser.parsing(_src, _label);

        ItemInfoCoinpart _data;
        int _itemcode;
        while (parser.next())
        {
            _data = new ItemInfoCoinpart();

            _itemcode = parser.getInt("itemcode");

            if (!dic_Base.ContainsKey(_itemcode))
            {
                _data.itemcode = _itemcode;                    //itemcode 아이템 코드는 중복되지 않는다, 고유 키 역할을 한다
                _data.category = parser.getInt("category");    //메인 카테고리로 장비템,소모품,기타템을 분류하는 역할을한다, 인벤토리 위치를 판단한다
                _data.subcategory = parser.getInt("subcategory"); //장비템 중에서도 무기,방어구 처럼 서브 종류를 말한다, 무기를 장착할때 서브카테고리가 중복되면 해당카테고리는 벗어난다
                _data.itemname = parser.getString("itemname"); //아이템 이름을 표기
                _data.gamecost = parser.getInt("gamecost");    //돈, 사냥을하거나 물건을 팔거나 퀘스트를깨면 받는 돈
                _data.cashcost      = parser.getInt("cashcost");  //캐쉬
                _data.buyamount = parser.getInt("buyamount");   //아이템 갯수,수량 
                _data.sellcost = parser.getInt("sellcost");    //아이템 판매할때 금액

                dic_Base.Add(_itemcode, _data);
                dic_CoinPart.Add(_itemcode, _data);
            }
        }
    }
    #endregion

}

