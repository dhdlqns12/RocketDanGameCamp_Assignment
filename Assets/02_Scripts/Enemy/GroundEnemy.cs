namespace StarDefense.Enemy
{
    /// <summary>
    /// 지상 적 유닛. 지대지 영웅만 공격 가능.
    /// </summary>
    public class GroundEnemy : Enemy
    {
        public override bool IsAir => false;
    }
}
