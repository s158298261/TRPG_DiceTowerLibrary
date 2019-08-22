# TRPG_DiceTowerLibrary
## a DiceTower for TRPG

### 使用：

**骰子**：     
aDb  *-a:骰子数量   -b:骰子面数*
<br>aDbUc  *-U:保留最大的c个D面骰子*
<br>aDbLc  *-L:保留最小的c个D面骰子*
<br>**例子**：     

* *公式投骰*：
<br>可进行*投骰*与*四则运算*混合使用，支持括号。  *仅支持小括号()，不支持[]{}*
<br>支持自定义随机种子。
<br>```DiceTower diceTower = new DiceTower(seed);```
<br>有输入检测，如果错误会抛出异常，可放心使用。
           
           DiceTower.Roll("1+2*3/4-4D6");
           DiceTower.Roll("10D20L4");
           DiceTower.Roll("((1d4+3*2)*3+8/(4d6-2*3))");
           
         
* *简单投骰*：
<br>仅可进行投骰运算
<br>**没有**输入检测
<br>建议仅在输入完全有保障（如按钮）的情况下使用。
           
           DiceTower.RollDices("3D6U1");
