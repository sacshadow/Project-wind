using UnityEngine;
using System.Collections;
//点击的物体{任何, 无, 玩家, 敌人, 事件, 错误}
public enum ClickedObject{ANY=0,EMPTY, CHARACTER, ENEMY, EVENT,NULL}
//距离目标{格斗距离, 冲锋距离, 远距离}
public enum DisToTarget{CLOSECOMBAT,CHARGE,RANGE}
//方向{任何, 前, 后, 左, 右}
public enum Side{ANY=0,FRONT,BACK,LEFT,RIGHT}
//输入方向{无, 上, 右上, 右, 右下, 下, 左下, 左, 左上, 任何}
public enum InputDirection{NONE=0,UP,UPRIGHT,RIGHT,DOWNRIGHT,DOWN,DOWNLEFT,LEFT,UPLEFT,ANY}
