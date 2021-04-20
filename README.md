# 炉石兄弟个人策略分享

#### 介绍
炉石兄弟吧 4.1 版本兄弟个人策略分享，仅吧友交流使用，侵删。

仅供学习免费使用，严禁贩卖。

非自用版本，如遇到各类 bug 或卡牌不全问题请自行解决或提问，概不负责后续维护。

使用任意一套卡组请在自定义策略中切换到对应的卡组策略。

#### 使用说明
策略完全基于炉石兄弟吧 4.1 版本兄弟。

使用请删除 \Routines\DefaultRoutine 目录，将下载得到的 DefaultRoutine 复制到原 \Routines 目录下

请尽量使用英雄初始皮肤（非异画英雄技能）和初始幸运币，尽量不使用金卡。

#### 当前支持卡组

#####  奥秘法
狂野套牌：AAEBAf0ECMAB67oC2bsCv6QDkOEDleEDwfAD558EC7sC7AX3Dde2Aoe9AsHBAo/TAr6kA92pA/SrA5HhAwA=

来源：https://www.iyingdi.com/web/tools/hearthstone/userdecks/deckdetail/6144457?btypes=home

#####  封印骑
狂野套牌：AAEBAZ8FBKcF3QqOmgPj6wMNjAGIDuoPnRX40gLZ/gLKwQOD3gOR5AP36APO6wOanwTJoAQA

来源：https://hsreplay.net/decks/3qZldXDg3OuWN3WBPO58Pb/

#####  偶数萨（未测试）

#####  小丑德
标准套牌：AAECAZICApXkA6iKBA7ougOVzQO60AO80AOT0QPe0QPw1AP+2wPm4QP36AO+7APe7AOJnwSunwQA

#####  标准模式骑士
策略选择：标准骑士
新的快攻骑：AAECAYsWBPy4A47UA+fwA5qfBA37uAPKwQOezQO/0QPK0QPg0QOL1QOF3gOR5APM6wPj6wPrnwTJoAQA
快攻：AAECAZ8FBITBA8DRA6iKBMigBA3KuAP9uAPquQPruQPsuQPKwQOezQOfzQOD3gPM6wPO6wPP6wPb7gMA
圣契：AAECAZ8FBo7UA4XeA4DsA9vuA5qfBOqfBAzKwQOezQPg0QOD3gOR5APM6wPO6wPP6wPj6wOr7APIoATJoAQA
奥秘：AAECAYsWBo7UA4XeA4DsA9vuA+fwA+qfBAzKwQOezQPg0QOD3gOR5APM6wPO6wPP6wPj6wOr7APIoATJoAQA

#### 添加自定义策略（惩罚）简易教程
1. 复制文件夹 Routines\DefaultRoutine\Silverfish\behavior\rush 到同级目录
2. 修改 Behavior怼脸模式.cs 文件中 BehaviorRush 和 "怼脸模式" 为任意其他名称，比如想要组建鱼人萨的策略就可以把 BehaviorRush 替换为 Behavior鱼人萨，同时可以顺便把文件中 "怼脸模式" 修改为 "鱼人萨"
3. 在 GetSpecialCardComboPenalty 函数中修改你的惩罚
4. 配置 _mulligan.txt，设置留牌策略
5. 打开兄弟，在 配置->策略->策略模式切换 中选择你新加入的策略
- 如有文件配置不理解，各级目录说明参考 http://blog.wjhwjhn.com/archives/16/
- GetSpecialCardComboPenalty 函数中返回值为负数表示鼓励兄弟出这张卡，正数表示不推荐出这张卡，值越大程度越高，经典惩罚配置例子参考 https://www.cnblogs.com/dch0319/p/13353571.html
- 卡基本上都有（指自动生成的空卡），配置惩罚的时候直接在 case "这里写需要加惩罚的卡牌的中文名称": 里面写就行
