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
            //Debug.Log(m);


            GameManager.instance.TaskQueue.Enqueue(new Action(() => {
                GameManager.instance.Entities.TryGetValue(m.GetEntityId(), out Entity e);
                Player p = (Player)e.gameObject.GetComponent<Player>();
                
                if(GameManager.instance.currentPlayer != null && GameManager.instance.currentPlayer.ID == m.GetEntityId())
                {
                    return;
                }

                if (Vector2.Distance(p.transform.position, new Vector2(m.GetXCoordinate(), m.GetYCoordinate())) > 2)
                {
                    p.DirectMove(m.GetXCoordinate(), m.GetYCoordinate(), m.GetXVelocity(), m.GetYVelocity());

                    if (m.GetSequenceNumber() > p.LastUpdate)
                    {
                        p.DirectMove(m.GetXCoordinate(), m.GetYCoordinate(), m.GetXVelocity(), m.GetYVelocity());
                    }
                    else if (Math.Abs(m.GetSequenceNumber() - p.LastUpdate) > UInt16.MaxValue / 4)
                    {
                        p.DirectMove(m.GetXCoordinate(), m.GetYCoordinate(), m.GetXVelocity(), m.GetYVelocity());
                    }
                }

                if (GameManager.instance.currentPlayer == null || GameManager.instance.currentPlayer.ID != m.GetEntityId())
                {
                    p.DirectMove(m.GetXCoordinate(), m.GetYCoordinate(), m.GetXVelocity(), m.GetYVelocity());
                }

                p.LastUpdate = m.GetSequenceNumber();
            }));
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
                if(GameManager.instance.Entities.ContainsKey(m.GetEntityID()))
                {
                    return;
                }

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
                    p.Controllable = true;
                    GameManager.instance.currentPlayer = p;

                    p.gameObject.AddComponent<InputHandler>();
                    GameObject.FindGameObjectWithTag("MainCamera").AddComponent<Follow>();
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Follow>().SetTarget(p.gameObject);
                }));
            }
        }

        public void Visit(Message m) { Debug.Log(m); }
    }
}
