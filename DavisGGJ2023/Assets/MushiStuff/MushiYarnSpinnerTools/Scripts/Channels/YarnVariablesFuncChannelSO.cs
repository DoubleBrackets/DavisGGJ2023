using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Channels/Funcs/YarnVariablesFuncChannel", fileName = "NewYarnVariablesFuncChannel")]
public class YarnVariablesFuncChannelSO : GenericFuncChannelSO<
    (Dictionary<string, float>,
    Dictionary<string, string>,
    Dictionary<string, bool>)>
{
    
}
