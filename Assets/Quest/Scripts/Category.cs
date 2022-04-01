using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Category", fileName ="Category_")]
public class Category : ScriptableObject, IEquatable<Category>
{
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;
    public string CodeName => codeName;
    public string DisplayName => displayName;

    #region Operator
    public bool Equals(Category other){
        if(other is null){
            return false;
        }
        if(ReferenceEquals(other,this)){//Reference와 other가 같다면
            return true;
        }
        if(GetType() != other.GetType()){
            return false;
        }
        return codeName == other.codeName;
    }
    public override int GetHashCode() => (CodeName,DisplayName).GetHashCode();
    public override bool Equals(object other) => base.Equals(other);
    
    public static bool operator == (Category lhs,string rhs){
        if(lhs is null){
            return ReferenceEquals(rhs,null);
        }
        return lhs.CodeName == rhs || lhs.DisplayName == rhs;
    }
    public static bool operator != (Category lhs, string rhs) => !(lhs == rhs);
    //Category.Codename == "Kill"로 선언할 필요 없이 Category == "Kill";로 선언하면 된다.
    #endregion
}