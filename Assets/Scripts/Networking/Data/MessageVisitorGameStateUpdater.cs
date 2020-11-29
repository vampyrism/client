using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Server
{
    class MessageVisitorGameStateUpdater : IMessageVisitor
    {
        public void Visit(MovementMessage m)
        {
            Debug.Log(m);

            GameManager.instance.Entities.TryGetValue(m.GetEntityId(), out Entity e);
            Player p = (Player)e;
        }

        public void Visit(AttackMessage m)
        {
            Debug.Log(m);
        }

        public void Visit(EntityUpdateMessage m)
        {
            Debug.Log(m);
            if(m.GetEntityAction() == EntityUpdateMessage.Action.CREATE)
            {
                if(m.GetEntityType() == EntityUpdateMessage.Type.PLAYER)
                {
                    GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                        // Refactor out!
                        Player p = (Player) GameObject.Instantiate(Resources.Load<GameObject>("Player")).GetComponent<Player>();
                        p.ID = m.GetEntityID();
                        GameManager.instance.Entities.TryAdd(p.ID, p);
                    }));
                }
            }
            else if(m.GetEntityAction() == EntityUpdateMessage.Action.CONTROL)
            {
                GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                    GameManager.instance.Entities.TryGetValue((UInt32)m.GetEntityID(), out Entity e);
                    Player p = (Player)e;

                    GameObject.FindGameObjectWithTag("MainCamera").AddComponent<Follow>();
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().SetTarget(p.gameObject);
                }));
            }
        }

        public void Visit(Message m) { Debug.Log(m); }
    }
}
