using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollider
{
    /// <summary>
    /// 检查两个凸多边形是否重合
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CheckPolygonAndPolygon(CustomPolygonCollider a, CustomPolygonCollider b)
    {
        //障碍物之间的碰撞不处理
        if (a.mIsobstacle && b.mIsobstacle)
            return false;

        List<CustomVector3> a_worldBound = a.LocalToWorldBound;
        List<CustomVector3> b_worldBound = b.LocalToWorldBound;

        bool mInit = false;
        //a在左
        bool a_at_left = true;
        //偏移向量
        CustomVector2 offsetvec = new CustomVector2(0, 0);
        //偏移值
        FixedPointF offsetfac = new FixedPointF(0);

        //顶点连线，
        //斜率k
        List<CustomVector2> a_edges = new List<CustomVector2>();
        for (int i = 0; i < a_worldBound.Count; i++)
        {
            int point1_index = i;
            int point2_index = (i + 1) % a_worldBound.Count;
            CustomVector3 offset = a_worldBound[point2_index] - a_worldBound[point1_index];
            a_edges.Add(new CustomVector2(offset.x, offset.z));
        }
        for (int i = 0; i < a_edges.Count; i++)
        {
            CustomVector2 axis = a_edges[i];
            //获得法向量
            axis = Vec_normal(axis);

            FixedPointF x = axis.x;
            FixedPointF y = axis.y;
            FixedPointF temp = x * x + y * y;
            FixedPointF z = FixedPointF.Sqrt(temp);
            axis.x = x / z;
            axis.y = y / z;

            CustomVector2 proj_a = GetMapPointMinMaxDis(a_worldBound, axis), proj_b = GetMapPointMinMaxDis(b_worldBound, axis);

            if (!Check_Overlap(proj_a, proj_b))
                return false;
            else
            {
                bool a_at_left_temp = false;
                FixedPointF offsetfac_temp = new FixedPointF(0);
                Set_PushVec(proj_a, proj_b, ref a_at_left_temp, ref offsetfac_temp);

                if (!mInit)
                {
                    a_at_left = a_at_left_temp;
                    offsetfac = offsetfac_temp;
                    offsetvec = axis;
                    mInit = true;
                }
                else
                {
                    if (offsetfac_temp < offsetfac)
                    {
                        a_at_left = a_at_left_temp;
                        offsetfac = offsetfac_temp;
                        offsetvec = axis;
                    }
                }
            }
        }

        //顶点连线
        //斜率k
        List<CustomVector2> b_edges = new List<CustomVector2>();
        for (int i = 0; i < b_worldBound.Count; i++)
        {
            int point1_index = i;
            int point2_index = (i + 1) % b_worldBound.Count;
            CustomVector3 offset = b_worldBound[point2_index] - b_worldBound[point1_index];
            b_edges.Add(new CustomVector2(offset.x, offset.z));
        }
        for (int i = 0; i < b_edges.Count; i++)
        {
            CustomVector2 axis = b_edges[i];
            axis = Vec_normal(axis);

            FixedPointF x = axis.x;
            FixedPointF y = axis.y;
            FixedPointF temp = x * x + y * y;
            FixedPointF z = FixedPointF.Sqrt(temp);
            axis.x = x / z;
            axis.y = y / z;

            CustomVector2 proj_a = GetMapPointMinMaxDis(a_worldBound, axis), proj_b = GetMapPointMinMaxDis(b_worldBound, axis);

            if (!Check_Overlap(proj_a, proj_b))
                return false;
            {
                bool a_at_left_temp = false;
                FixedPointF offsetfac_temp = new FixedPointF(0);

                Set_PushVec(proj_a, proj_b, ref a_at_left_temp, ref offsetfac_temp);

                if (offsetfac_temp < offsetfac)
                {
                    a_at_left = a_at_left_temp;
                    offsetfac = offsetfac_temp;
                    offsetvec = axis;
                }
            }
        }
        Push(a, b, offsetfac, offsetvec, a_at_left);
        return true;
    }

    /// <summary>
    /// 检查凸多边形和圆是否重合
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CheckPolygonAndCircle(CustomPolygonCollider a, CustomCircleCollider b)
    {
        //障碍物之间的碰撞不处理
        if (a.mIsobstacle && b.mIsobstacle)
            return false;

        List<CustomVector3> a_worldBound = a.LocalToWorldBound;

        bool mInit = false;
        //a在左
        bool a_at_left = true;
        //偏移向量
        CustomVector2 offsetvec = new CustomVector2(0, 0);
        //偏移值
        FixedPointF offsetfac = new FixedPointF(0);

        //顶点连线
        //斜率k
        List<CustomVector2> a_edges = new List<CustomVector2>();
        for (int i = 0; i < a_worldBound.Count; i++)
        {
            int point1_index = i;
            int point2_index = (i + 1) % a_worldBound.Count;
            CustomVector3 offset = a_worldBound[point2_index] - a_worldBound[point1_index];
            a_edges.Add(new CustomVector2(offset.x, offset.z));
        }
        for (int i = 0; i < a_edges.Count; i++)
        {
            CustomVector2 axis = a_edges[i];
            axis = Vec_normal(axis);            

            FixedPointF x = axis.x;
            FixedPointF y = axis.y;
            FixedPointF temp = x * x + y * y;
            FixedPointF z = FixedPointF.Sqrt(temp);
            axis.x = x / z;
            axis.y = y / z;

            //求圆在法线上的投影边界
            CustomVector3 vec3 = b.mTrans.Position;
            CustomVector2 point = new CustomVector2(vec3.x, vec3.z);
            FixedPointF mapPoint = CustomVector2.Dot(point, axis);
            FixedPointF min = mapPoint - b.mRadius;
            FixedPointF max = mapPoint + b.mRadius;
            CustomVector2 proj_b = new CustomVector2(min, max);

            CustomVector2 proj_a = GetMapPointMinMaxDis(a_worldBound, axis);

            if (!Check_Overlap(proj_a, proj_b))
                return false;
            else
            {
                bool a_at_left_temp = false;
                FixedPointF offsetfac_temp = new FixedPointF(0);
                Set_PushVec(proj_a, proj_b, ref a_at_left_temp, ref offsetfac_temp);
                if (!mInit)
                {
                    a_at_left = a_at_left_temp;
                    offsetfac = offsetfac_temp;
                    offsetvec = axis;
                    mInit = true;
                }
                else
                {
                    if (offsetfac_temp < offsetfac)
                    {
                        a_at_left = a_at_left_temp;
                        offsetfac = offsetfac_temp;
                        offsetvec = axis;
                    }
                }
            }
        }
        Push(a, b, offsetfac, offsetvec, a_at_left);
        return true;
    }

    /// <summary>
    /// 检查凸两个圆是否重合
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CheckCircleAndCircle(CustomCircleCollider a, CustomCircleCollider b)
    {
        //障碍物之间的碰撞不处理
        if (a.mIsobstacle && b.mIsobstacle)
            return false;

        bool mInit = false;
        //a在左
        bool a_at_left = true;
        //偏移向量
        CustomVector2 offsetvec = new CustomVector2(0, 0);
        //偏移值
        FixedPointF offsetfac = new FixedPointF(0);

        //圆心连线
        CustomVector3 vec3 = a.mTrans.Position - b.mTrans.Position;
        CustomVector2 axis = new CustomVector2(vec3.x, vec3.z);

        FixedPointF x = axis.x;
        FixedPointF y = axis.y;
        FixedPointF temp = x * x + y * y;
        FixedPointF z = FixedPointF.Sqrt(temp);
        
        //圆心重合，x轴推开
        if (z == FixedPointF.zero)
        {
            axis.x = new FixedPointF(1000, 1000);
            axis.y = new FixedPointF(0, 1000);
        }
        else
        {
            axis.x = x / z;
            axis.y = y / z;
        }


        CustomVector3 apos = a.mTrans.Position;
        CustomVector2 point = new CustomVector2(apos.x, apos.z);
        FixedPointF mapPoint = CustomVector2.Dot(point, axis);
        FixedPointF min = mapPoint - a.mRadius;
        FixedPointF max = mapPoint + a.mRadius;
        CustomVector2 proj_a = new CustomVector2(min, max);

        CustomVector3 bpos = b.mTrans.Position;
        point = new CustomVector2(bpos.x, bpos.z);
        mapPoint = CustomVector2.Dot(point, axis);
        min = mapPoint - b.mRadius;
        max = mapPoint + b.mRadius;
        CustomVector2 proj_b = new CustomVector2(min, max);

        if (!Check_Overlap(proj_a, proj_b))
            return false;
        else
        {
            bool a_at_left_temp = false;
            FixedPointF offsetfac_temp = new FixedPointF(0);
            Set_PushVec(proj_a, proj_b, ref a_at_left_temp, ref offsetfac_temp);

            if (!mInit)
            {
                a_at_left = a_at_left_temp;
                offsetfac = offsetfac_temp;
                offsetvec = axis;
                mInit = true;
            }
            else
            {
                if (offsetfac_temp < offsetfac)
                {
                    a_at_left = a_at_left_temp;
                    offsetfac = offsetfac_temp;
                    offsetvec = axis;
                }
            }
        }
        Push(a, b, offsetfac, offsetvec, a_at_left);
        return true;
    }

    static void Push(CustomCollider a, CustomCollider b, FixedPointF offsetfac, CustomVector2 offsetvec, bool a_at_left)
    {
        FixedPointF fac = offsetfac;

        CustomVector3 offset_pos;
        offset_pos.x = fac * offsetvec.x;
        offset_pos.y = new FixedPointF(0);
        offset_pos.z = fac * offsetvec.y;

        //Actor actor = null;
        //bool actorisA = false;
        //if (a.mIsobstacle && !b.mIsobstacle)
        //    actor = b.GetComponentInParent<Actor>();
        //else if (!a.mIsobstacle && b.mIsobstacle)
        //{
        //    actor = a.GetComponentInParent<Actor>();
        //    actorisA = true;
        //}
        //if (actor != null)
        //{
        //    if (a_at_left == actorisA)
        //        actor.Position -= offset_pos;
        //    else
        //        actor.Position += offset_pos;
        //}

        if (!a.mIsobstacle && !b.mIsobstacle)
            return;

        if (a.mIsobstacle && !b.mIsobstacle)
        {
            if (a_at_left)
                b.AddOffsetPosition(offset_pos);
            else
                b.AddOffsetPosition(-offset_pos);
        }
        else if (!a.mIsobstacle && b.mIsobstacle)
        {
            if (a_at_left)
                a.AddOffsetPosition(-offset_pos);
            else
                a.AddOffsetPosition(offset_pos);
        }
    }

    /// <summary>
    /// 获得一条直线的法向量
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    static CustomVector2 Vec_normal(CustomVector2 vec)
    {
        if (vec.y == FixedPointF.zero)
            return new CustomVector2(0, 1);
        //法线向量
        return new CustomVector2(-vec.y, vec.x);
    }

    /// <summary>
    /// 获得多边形在一条直线上的投影范围
    /// </summary>
    /// <param name="list"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    static CustomVector2 GetMapPointMinMaxDis(List<CustomVector3> list, CustomVector2 axis)
    {
        CustomVector2 point = new CustomVector2(list[0].x, list[0].z);
        FixedPointF min = CustomVector2.Dot(point, axis);
        FixedPointF max = min;
        for (int i = 1; i < list.Count; i++)
        {
            point = new CustomVector2(list[i].x, list[i].z);
            FixedPointF doc = CustomVector2.Dot(point, axis);
            if (doc < min)
                min = doc;
            if (doc > max)
                max = doc;
        }
        point.x = min;
        point.y = max;
        return point;
    }

    /// <summary>
    /// 检查两个多边形在同一条直线上的投影是否有重合
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <returns></returns>
    static bool Check_Overlap(CustomVector2 vec1, CustomVector2 vec2)
    {
        if (vec1.y < vec2.x || vec2.y < vec1.x)
            return false;
        else
            return true;
    }

    /// <summary>
    /// 设置推开向量
    /// </summary>
    /// <param name="proj_a"></param>
    /// <param name="proj_b"></param>
    /// <param name="a_at_left_temp"></param>
    /// <param name="offsetfac_temp"></param>
    static void Set_PushVec(CustomVector2 proj_a, CustomVector2 proj_b, ref bool a_at_left_temp, ref FixedPointF offsetfac_temp)
    {
        if (proj_a.x == proj_b.x)
        {
            if (proj_a.y <= proj_b.y)
            {
                a_at_left_temp = true;
                offsetfac_temp = proj_a.y - proj_a.x;
            }
            else
            {
                a_at_left_temp = false;
                offsetfac_temp = proj_b.y - proj_a.x;
            }
        }
        else if (proj_a.x < proj_b.x)
        {
            a_at_left_temp = true;
            offsetfac_temp = proj_a.y - proj_b.x;
        }
        else
        {
            a_at_left_temp = false;
            offsetfac_temp = proj_b.y - proj_a.x;
        }
    }
}
