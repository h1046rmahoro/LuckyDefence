using UnityEngine;

namespace SB
{
    public class Bezier
    {
        /// <summary>
        /// 베지어 곡선 위치를 구하는 함수 
        /// </summary>
        /// <returns>베지어 곡선상위 위치값 </returns>
        /// <param name="p">베지어 곡선의 베이스가 되는 위치값들 </param>
        /// <param name="mu">0부터 1까지의 진행정도 </param>
        public static Vector3 GetPosition(Vector3[] p, float mu)
        {
            return GetPosition(p, p.Length - 1, mu);
        }

        /// <summary>
        /// 베지어 곡선 위치를 구하는 함수 
        /// </summary>
        /// <returns>베지어 곡선상위 위치값</returns>
        /// <param name="p">베지어 곡선의 베이스가 되는 위치값들 </param>
        /// <param name="n">곡선의 베이스가 되는 위치값의 개수 -1</param>
        /// <param name="mu">0부터 1까지의 진행정도</param>
        public static Vector3 GetPosition(Vector3[] p,int n,float mu)
        {
            if(mu >= 1)
            {
                return p[n];
            }

            int k,kn,nn,nkn;
            float blend,muk,munk;
            Vector3 b = Vector3.zero;
            muk = 1;
            munk = Mathf.Pow(1-mu,(float)n);
            for (k=0;k<=n;++k)
            {
                nn = n;
                kn = k;
                nkn = n - k;
                blend = muk * munk;
                muk *= mu;
                munk /= (1-mu);
                while (nn >= 1) {
                    blend *= nn;
                    nn--;
                    if (kn > 1) {
                        blend /= (float)kn;
                        kn--;
                    }
                    if (nkn > 1) {
                        blend /= (float)nkn;
                        nkn--;
                    }
                }
                b.x += p[k].x * blend;
                b.y += p[k].y * blend;
                b.z += p[k].z * blend;
            }
            return(b);
        }

        /// <summary>
        /// 베지어 곡선을 그리는 디버그용 함수
        /// </summary>
        /// <param name="p">베지어 곡선의 베이스가 되는 위치값들 </param>
        /// <param name="color">베지어 곡선의 색 </param>
        /// <param name="duration">베지어 곡선의 지속 시간 </param>

        public static void drawLine(Vector3[] p, Color color, float duration)
        {
    #if UNITY_EDITOR
            for (int i = 0; i < 30; ++i)
            {
                Debug.DrawLine(GetPosition(p, i / 30.0f), GetPosition(p, (i + 1) / 30.0f), color, duration);
            }
    #endif
        }

    }
}
