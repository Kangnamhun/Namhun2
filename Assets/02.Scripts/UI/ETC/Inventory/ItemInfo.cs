using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ItemInfo : MonoBehaviour
{
    //�������̳� UI�� ǥ���� �̹��� ������ ����ִ�
    //���� : atlas�� �̸�(string)�� �־��ָ� �ش� sprite�� ���ϵȴ�
    public SpriteAtlas attlas;
    #region sigletone
    public static ItemInfo ins;
    private void Awake()
    {
        ins = this;
        ReadAndParse();
        //������ �����ʹ� �ٸ� ��ͺ��� ���� ������ �Ǿ�� �ϹǷ� Awake���� ����
    }
    #endregion
    //���� �����Ͱ� ����ִºκ�
    public Dictionary<int, ItemInfoBase> dic_Base = new Dictionary<int, ItemInfoBase>();

    //������� ���� ������ ����ִ� �κ� + ����
    public Dictionary<int, ItemInfoWearPart> dic_WearPart = new Dictionary<int, ItemInfoWearPart>();

    //�Һ����� ���� ������ ����ִ� �κ� + ����
    public Dictionary<int, ItemInfoUsepart> dic_UsePart = new Dictionary<int, ItemInfoUsepart>();

    //��Ÿ���� ���� ������ ����ִ� �κ� + ����
    public Dictionary<int, ItemInfoETCpart> dic_ETCPart = new Dictionary<int, ItemInfoETCpart>();

    
    public Dictionary<int, ItemInfoCoinpart> dic_CoinPart = new Dictionary<int, ItemInfoCoinpart>();


    SSParser parser = new SSParser(); //XML(string)�����͸� �м����ִ� Ŭ����
    string strItemInfo;


    public void ReadAndParse()
    {
        //XML file�����͸� �о string ������ �־���
        strItemInfo = SSUtil.load("txt/iteminfo"); //txt������ iteminfo�� �д´�

        //string ������ �ִ� �������߿� �ش� lable(�̸�)���� �Ȱ��� �м��ؼ� class�� ������ dic�� ����Ѵ�
        ParseWearPart(strItemInfo, "wearpart");     //ParseWearPart ���� wearpart �鸸 �̾Ƴ���(��� ������)
        ParseUsePart(strItemInfo, "usepart");      //ParseWearPart ���� usepart �鸸 �̾Ƴ���(�Һ� ������)
        ParseETCPart(strItemInfo, "etcpart");      //ParseWearPart ���� etcpart �鸸 �̾Ƴ���(��Ÿ ������)
        ParseCoinPart(strItemInfo, "coinpart");     
    }

    #region common data (���� ������)
    public string GetItemInfoIcon(int _itemcode)
    {
        //itemcode �� ��ü dic�� �˻��ؼ� icon(string)�̸��� ã�ƿ���
        string _rtn = null;

        //itemcode�� dic�� �ִ°�
        if (dic_Base.ContainsKey(_itemcode))
        {
            //������ �ش������ �̸��� _rtn�� �־��ֱ�
            _rtn = dic_Base[_itemcode].icon;
        }
        return _rtn;
    }

    public Sprite GetItemInfoSpriteIcon(int _itemcode) //������ �ڵ�� �ҷ����°�
    {
        //itemcode�� ������ icon�̸��� ã���� �װ��� atlas���� �̸�(string)���� �˻��ؼ� sprite�� ã�ƿ´�
        return attlas.GetSprite(GetItemInfoIcon(_itemcode));
    }

    public Sprite GetSprite(string _itemName) //������ �̸����� �ҷ����°�
    {
        return attlas.GetSprite(_itemName);
    }

    public ItemInfoBase GetItemInfoBase(int _itemcode)
    {
        //itemcode�� �˻��ؼ� base�κп� Ŭ���� �����͸� ã����
        //base : itemcode.subcategory ~ description���� ����ִ� Ŭ���� ������
        ItemInfoBase _rtn = null;

        //itemcode�� dic�� �ִ°�
        if (dic_Base.ContainsKey(_itemcode))
        {
            _rtn = dic_Base[_itemcode];
        }
        return _rtn;
    }
    #endregion

    #region wearpart
    //itemcode�� �־��ָ� wearpart���ִ� dic���� �ش� �������� �˻����ش�.
    //�������� �ִٸ� �ش� ������ class�� ����
    //�������� ���ٸ� null�� ����
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
        //XML�� �ִ� WearPart��Ʈ��� �̸����� �Ǿ��ִ°͵��� ���� �м��Ѵ�.
        parser.parsing(_src, _label);

        ItemInfoWearPart _data;
        int _itemcode;
        while (parser.next())//parser.next�� ������ 
        {
            //���� Ŀ���� ��ġ�� �ִ� ������ ������ �м� �Ҽ� �ֵ��� Ŀ��(����Ʈ)�� ��ġ�Ѵ�.
            //ù��° ���� Ŭ������ �ϳ� �����д�.
            _data = new ItemInfoWearPart();

            //�����۵��� ��δ� itemcode�� �����Ǹ� ������ itemcode�� ����ǰ� �����ǹǷ� �̰��� Ű���ȴ�.
            //���� ������ Ŭ������ �����ϴ� Dictioary������ itemcode�� �����ǰ� �˻��ȴ�.
            _itemcode = parser.getInt("itemcode");

            //������ �ڵ�� �˻��ؼ� ������ �̹� �Ľ��߱⶧���� 2��°�� ��Ͼ���
            //������ �ڵ�� �˻��ؼ� ������ �Ľ̵� �����͸� Ŭ������ �־ dictorary�� ������ش�.
            if (!dic_Base.ContainsKey(_itemcode))
            {
                //XML�� �ش��̸����� �˻��� �ؼ� �ش�Ŭ���� ������ �����ؼ� �־��ش�
                _data.itemcode      = _itemcode;                    //itemcode ������ �ڵ�� �ߺ����� �ʴ´�, ���� Ű ������ �Ѵ�
                _data.category      = parser.getInt("category");    //���� ī�װ��� �����,�Ҹ�ǰ,��Ÿ���� �з��ϴ� �������Ѵ�, �κ��丮 ��ġ�� �Ǵ��Ѵ�
                _data.subcategory   = parser.getInt("subcategory"); //����� �߿����� ����,�� ó�� ���� ������ ���Ѵ�, ���⸦ �����Ҷ� ����ī�װ��� �ߺ��Ǹ� �ش�ī�װ��� �����
                _data.equpslot      = parser.getInt("equpslot");    //��� ���� ��ġ
                _data.itemname      = parser.getString("itemname"); //������ �̸��� ǥ��
                _data.activate      = parser.getInt("activate");    //������ Ȱ��ȭ ����
                //_data.toplist       = parser.getInt("toplist");     
                _data.grade         = parser.getInt("grade");       //�������� ���(�Ϲ�,����,����)
                //_data.discount      = parser.getInt("discount");    �������� ������
                _data.icon          = parser.getString("icon");     //������ �������� ǥ���Ѵ�
                _data.playerlv      = parser.getInt("playerlv");    //�������� ��������
                //_data.multistate    = parser.getInt("multistate");  ��Ʈ������
                _data.gamecost      = parser.getInt("gamecost");    //��, ������ϰų� ������ �Ȱų� ����Ʈ������ �޴� ��
                //_data.cashcost      = parser.getInt("cashcost");  ĳ��
                _data.buyamount     = parser.getInt("buyamount");   //������ ����,���� 
                _data.sellcost      = parser.getInt("sellcost");    //������ �Ǹ��Ҷ� �ݾ�
                _data.description   = parser.getString("description");  

                _data.plusatt   = parser.getFloat("plusatt");           // ��� ���ݷ�, �÷��� ���ݷ�
                _data.plusdef   = parser.getFloat("plusdef");           // ��� ����, �÷��� ����
                _data.plushp    = parser.getFloat("plushp");             // ��� hp, �÷��� hp
                _data.plusmp    = parser.getFloat("plusmp");             // ��� mp, �÷��� mp
                _data.skin      = parser.getString("skin");              //��� Ű�°�
                _data.skin2     = parser.getString("skin2");


                //������� �Ѱ��� ��� ������ Ŭ������ ������ Dic�� �־��ش�
                //�� ������ dic�� �����͸� �˻��ؼ� ����Ѵ�
                //������ �ڵ�� �˻��ؼ� ���� Ŭ����(������ ������)�� �ش� itemcode�� ��� �����Ͱ� ����־ ������ �����۵��� ǥ���Ѵ�\
                // ��) ������ ���� ����Ҷ�  itemcode -> dic -> class.Plusdef�� ��� �����̴�
                // ��) ������ ����Ҷ� itemcode -> dic -> class.icon -> ������ ��������Ʈ �̸��� ����, �̰��� atlas �� �־����� �ٷ� ������sprite�� ����.
                dic_Base.Add(_itemcode, _data);
                dic_WearPart.Add(_itemcode, _data);
            }
        }
    }
    #endregion


    #region usepart
    //itemcode�� �־��ָ� Usepart���ִ� dic���� �ش� �������� �˻����ش�.
    //�������� �ִٸ� �ش� ������ class�� ����
    //�������� ���ٸ� null�� ����,  
    public ItemInfoUsepart GetItemInfoUsepart(int _itemcode)
    {
        ItemInfoUsepart _rtn = null;

        //dic_wearpart �ȿ� itemcode�� ����(����)�ϴ°�?
        if (dic_UsePart.ContainsKey(_itemcode))
        {
            //�����ϸ� dic�� �迭ó�� ����ϴµ� �迭�ȿ��� Ű���� �־��ָ� �ش� �����Ͱ� �˻��Ǿ ����
            //_rtn �� ���� Ŭ���� �����Ͱ� �������
            _rtn = dic_UsePart[_itemcode];
        }
        return _rtn;
    }

    //XML���� �ش� �̸����� �Ȱ͵��� �Ľ�(�м�)�ؼ� dic�� �־��ִ� �κ�.
    void ParseUsePart(string _src, string _label)
    {
        //��Ʈ �̸����� Parsing
        parser.parsing(_src, _label);

        ItemInfoUsepart _data;
        int _itemcode;
        while (parser.next())//parser.next�� ������ 
        {
            //���� Ŀ���� ��ġ�� �ִ� ������ ������ �м� �Ҽ� �ֵ��� Ŀ��(����Ʈ)�� ��ġ�Ѵ�.
            //ù��° ���� Ŭ������ �ϳ� �����д�.
            _data = new ItemInfoUsepart();

            //�����۵��� ��δ� itemcode�� �����Ǹ� ������ itemcode�� ����ǰ� �����ǹǷ� �̰��� Ű���ȴ�.
            //���� ������ Ŭ������ �����ϴ� Dictioary������ itemcode�� �����ǰ� �˻��ȴ�.
            _itemcode = parser.getInt("itemcode");

            if (!dic_Base.ContainsKey(_itemcode))
            {
                //XML�� �ش��̸����� �˻��� �ؼ� �ش�Ŭ���� ������ �����ؼ� �־��ش�
                _data.itemcode = _itemcode;                    //itemcode ������ �ڵ�� �ߺ����� �ʴ´�, ���� Ű ������ �Ѵ�
                _data.category = parser.getInt("category");    //���� ī�װ��� �����,�Ҹ�ǰ,��Ÿ���� �з��ϴ� �������Ѵ�, �κ��丮 ��ġ�� �Ǵ��Ѵ�
                _data.subcategory = parser.getInt("subcategory"); //����� �߿����� ����,�� ó�� ���� ������ ���Ѵ�, ���⸦ �����Ҷ� ����ī�װ��� �ߺ��Ǹ� �ش�ī�װ��� �����
                _data.equpslot = parser.getInt("equpslot");    //��� ���� ��ġ
                _data.itemname = parser.getString("itemname"); //������ �̸��� ǥ��
                _data.activate = parser.getInt("activate");    //������ Ȱ��ȭ ����
                //_data.toplist       = parser.getInt("toplist");     
                _data.grade = parser.getInt("grade");       //�������� ���(�Ϲ�,����,����)
                //_data.discount      = parser.getInt("discount");    �������� ������
                _data.icon = parser.getString("icon");     //������ �������� ǥ���Ѵ�
                _data.playerlv = parser.getInt("playerlv");    //�������� ��������
                //_data.multistate    = parser.getInt("multistate");  ��Ʈ������
                _data.gamecost = parser.getInt("gamecost");    //��, ������ϰų� ������ �Ȱų� ����Ʈ������ �޴� ��
                //_data.cashcost      = parser.getInt("cashcost");  ĳ��
                _data.buyamount = parser.getInt("buyamount");   //������ ����,���� 
                _data.sellcost = parser.getInt("sellcost");    //������ �Ǹ��Ҷ� �ݾ�
                _data.description = parser.getString("description");

                _data.hp = parser.getInt("hp"); //�Һ������ hp ȸ��
                _data.mp = parser.getInt("mp"); //�Һ������ mpȸ��
                

                //dic�� Add
                dic_Base.Add(_itemcode, _data);
                dic_UsePart.Add(_itemcode, _data);
            }
        }
    }
    #endregion

    #region ETCpart
    //itemcode�� �־��ָ� Usepart���ִ� dic���� �ش� �������� �˻����ش�.
    //�������� �ִٸ� �ش� ������ class�� ����
    //�������� ���ٸ� null�� ����, 
    public ItemInfoETCpart GetItemInfoETCPart(int _itemcode)
    {
        ItemInfoETCpart _rtn = null;

        //dic_wearpart �ȿ� itemcode�� ����(����)�ϴ°�?
        if (dic_ETCPart.ContainsKey(_itemcode))
        {
            //�����ϸ� dic�� �迭ó�� ����ϴµ� �迭�ȿ��� Ű���� �־��ָ� �ش� �����Ͱ� �˻��Ǿ ����
            //_rtn �� ���� Ŭ���� �����Ͱ� �������
            _rtn = dic_ETCPart[_itemcode];
        }
        return _rtn;
    }

    //XML���� �ش� �̸����� �Ȱ͵��� �Ľ�(�м�)�ؼ� dic�� �־��ִ� �κ�.
    void ParseETCPart(string _src, string _label)
    {
        //��Ʈ �̸����� Parsing
        parser.parsing(_src, _label);

        ItemInfoETCpart _data;
        int _itemcode;
        while (parser.next())//parser.next�� ������ 
        {
            //���� Ŀ���� ��ġ�� �ִ� ������ ������ �м� �Ҽ� �ֵ��� Ŀ��(����Ʈ)�� ��ġ�Ѵ�.
            //ù��° ���� Ŭ������ �ϳ� �����д�.
            _data = new ItemInfoETCpart();

            //�����۵��� ��δ� itemcode�� �����Ǹ� ������ itemcode�� ����ǰ� �����ǹǷ� �̰��� Ű���ȴ�.
            //���� ������ Ŭ������ �����ϴ� Dictioary������ itemcode�� �����ǰ� �˻��ȴ�.
            _itemcode = parser.getInt("itemcode");

            if (!dic_Base.ContainsKey(_itemcode))
            {
                //XML�� �ش��̸����� �˻��� �ؼ� �ش�Ŭ���� ������ �����ؼ� �־��ش�
                _data.itemcode = _itemcode;                    //itemcode ������ �ڵ�� �ߺ����� �ʴ´�, ���� Ű ������ �Ѵ�
                _data.category = parser.getInt("category");    //���� ī�װ��� �����,�Ҹ�ǰ,��Ÿ���� �з��ϴ� �������Ѵ�, �κ��丮 ��ġ�� �Ǵ��Ѵ�
                _data.subcategory = parser.getInt("subcategory"); //����� �߿����� ����,�� ó�� ���� ������ ���Ѵ�, ���⸦ �����Ҷ� ����ī�װ��� �ߺ��Ǹ� �ش�ī�װ��� �����
                _data.equpslot = parser.getInt("equpslot");    //��� ���� ��ġ
                _data.itemname = parser.getString("itemname"); //������ �̸��� ǥ��
                _data.activate = parser.getInt("activate");    //������ Ȱ��ȭ ����
                //_data.toplist       = parser.getInt("toplist");     
                _data.grade = parser.getInt("grade");       //�������� ���(�Ϲ�,����,����)
                //_data.discount      = parser.getInt("discount");    �������� ������
                _data.icon = parser.getString("icon");     //������ �������� ǥ���Ѵ�
                _data.playerlv = parser.getInt("playerlv");    //�������� ��������
                //_data.multistate    = parser.getInt("multistate");  ��Ʈ������
                _data.gamecost = parser.getInt("gamecost");    //��, ������ϰų� ������ �Ȱų� ����Ʈ������ �޴� ��
                //_data.cashcost      = parser.getInt("cashcost");  ĳ��
                _data.buyamount = parser.getInt("buyamount");   //������ ����,���� 
                _data.sellcost = parser.getInt("sellcost");    //������ �Ǹ��Ҷ� �ݾ�
                _data.description = parser.getString("description");

                /*_data.plusatt = parser.getInt("plusatt"); ��Ÿ �������� �ʿ����
                _data.plusdef = parser.getInt("plusdef");
                _data.plushp = parser.getInt("plushp");*/

                //dic�� Add
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
                _data.itemcode = _itemcode;                    //itemcode ������ �ڵ�� �ߺ����� �ʴ´�, ���� Ű ������ �Ѵ�
                _data.category = parser.getInt("category");    //���� ī�װ��� �����,�Ҹ�ǰ,��Ÿ���� �з��ϴ� �������Ѵ�, �κ��丮 ��ġ�� �Ǵ��Ѵ�
                _data.subcategory = parser.getInt("subcategory"); //����� �߿����� ����,�� ó�� ���� ������ ���Ѵ�, ���⸦ �����Ҷ� ����ī�װ��� �ߺ��Ǹ� �ش�ī�װ��� �����
                _data.itemname = parser.getString("itemname"); //������ �̸��� ǥ��
                _data.gamecost = parser.getInt("gamecost");    //��, ������ϰų� ������ �Ȱų� ����Ʈ������ �޴� ��
                _data.cashcost      = parser.getInt("cashcost");  //ĳ��
                _data.buyamount = parser.getInt("buyamount");   //������ ����,���� 
                _data.sellcost = parser.getInt("sellcost");    //������ �Ǹ��Ҷ� �ݾ�

                dic_Base.Add(_itemcode, _data);
                dic_CoinPart.Add(_itemcode, _data);
            }
        }
    }
    #endregion

}

