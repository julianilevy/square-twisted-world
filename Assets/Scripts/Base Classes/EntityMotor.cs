using System.Collections;
using UnityEngine;

public class EntityMotor : GravityEntity
{
    [HideInInspector]
    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 directionalInput;

    protected bool allVerticalRaysTouchingObstacle;
    protected bool alreadyFallingFromMovingPlatformSlicing;

    private int _faceDirection = 1;

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false)
    {
        UpdateRayCastOrigins();
        collisions.Reset();
        collisions.previousVelocity = moveAmount;

        if (moveAmount.x != 0)
            _faceDirection = (int)Mathf.Sign(moveAmount.x);

        HorizontalCollisions(ref moveAmount);

        if (moveAmount.y != 0)
            VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);

        if (standingOnPlatform)
            collisions.down = true;
    }

    void HorizontalCollisions(ref Vector2 velocity)
    {
        float directionX = _faceDirection;
        float rayLenght = Mathf.Abs(velocity.x) + skinWidth;
        int raysTouchingObstacle = 0;
        int raysTouchingMovingPlatform = 0;

        if (Mathf.Abs(velocity.x) < skinWidth)
            rayLenght = skinWidth * 2;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin;
            if (directionX == -1)
                rayOrigin = raycastOrigins.bottomLeft;
            else
                rayOrigin = raycastOrigins.bottomRight;
            rayOrigin += transformVector2.up * (verticalRaySpacing * i);

            var hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLenght, collisionMask);
            if (hit)
            {
                if (hit.distance == 0)
                    continue;

                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLenght = hit.distance;

                if (directionX == -1)
                    collisions.left = true;
                if (directionX == 1)
                    collisions.right = true;
            }

            hit = Physics2D.Raycast(rayOrigin, transform.right * directionX, rayLenght * 2, collisionMask);
            if (hit)
            {
                if (hit.transform.gameObject.layer == K.LAYER_OBSTACLE)
                    raysTouchingObstacle++;

                if (hit.transform.gameObject.layer == K.LAYER_MOVINGPLATFORM)
                    raysTouchingMovingPlatform++;
            }
        }

        if (raysTouchingMovingPlatform >= 1 && raysTouchingMovingPlatform <= 2)
            alreadyFallingFromMovingPlatformSlicing = true;
        else
            alreadyFallingFromMovingPlatformSlicing = false;

        if (raysTouchingObstacle == verticalRayCount)
            allVerticalRaysTouchingObstacle = true;
        else
            allVerticalRaysTouchingObstacle = false;
    }

    void VerticalCollisions(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLenght = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin;
            if (directionY == -1)
                rayOrigin = raycastOrigins.bottomLeft;
            else
                rayOrigin = raycastOrigins.topLeft;
            rayOrigin += transformVector2.right * (horizontalRaySpacing * i + velocity.x);

            var hit = Physics2D.Raycast(rayOrigin, transform.up * directionY, rayLenght, collisionMask);
            if (hit)
            {
                if (hit.collider.tag == K.TAG_PASSABLE && hit.collider.gameObject.layer == K.LAYER_MOVINGPLATFORM)
                {
                    if (currentGravity == Gravity.UP || currentGravity == Gravity.DOWN)
                    {
                        if (hit.transform.GetComponent<MovingPlatform>() && hit.transform.GetComponent<MovingPlatform>().platformType == MovingPlatform.PlatformType.horizontal)
                        {
                            if (directionY == 1 || hit.distance == 0)
                                continue;
                            if (collisions.fallingThroughPlatform)
                                continue;
                            if (directionalInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                StartCoroutine(ResetFallingThroughPlatform());
                                continue;
                            }
                        }
                        else if (hit.transform.GetComponent<MovingPlatformVerticalSpikes>() && hit.transform.GetComponent<MovingPlatformVerticalSpikes>().platformType == MovingPlatformVerticalSpikes.PlatformType.horizontal)
                        {
                            if (directionY == 1 || hit.distance == 0)
                                continue;
                            if (collisions.fallingThroughPlatform)
                                continue;
                            if (directionalInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                StartCoroutine(ResetFallingThroughPlatform());
                                continue;
                            }
                        }
                        else if (hit.transform.GetComponent<MovingPlatformHorizontalSpikes>() && hit.transform.GetComponent<MovingPlatformHorizontalSpikes>().platformType == MovingPlatformHorizontalSpikes.PlatformType.horizontal)
                        {
                            if (directionY == 1 || hit.distance == 0)
                                continue;
                            if (collisions.fallingThroughPlatform)
                                continue;
                            if (directionalInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                StartCoroutine(ResetFallingThroughPlatform());
                                continue;
                            }
                        }
                    }
                    else if (currentGravity == Gravity.RIGHT || currentGravity == Gravity.LEFT)
                    {
                        if (hit.transform.GetComponent<MovingPlatform>() && hit.transform.GetComponent<MovingPlatform>().platformType == MovingPlatform.PlatformType.vertical)
                        {
                            if (directionY == 1 || hit.distance == 0)
                                continue;
                            if (collisions.fallingThroughPlatform)
                                continue;
                            if (directionalInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                StartCoroutine(ResetFallingThroughPlatform());
                                continue;
                            }
                        }
                        else if (hit.transform.GetComponent<MovingPlatformVerticalSpikes>() && hit.transform.GetComponent<MovingPlatformVerticalSpikes>().platformType == MovingPlatformVerticalSpikes.PlatformType.vertical)
                        {
                            if (directionY == 1 || hit.distance == 0)
                                continue;
                            if (collisions.fallingThroughPlatform)
                                continue;
                            if (directionalInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                StartCoroutine(ResetFallingThroughPlatform());
                                continue;
                            }
                        }
                        else if (hit.transform.GetComponent<MovingPlatformHorizontalSpikes>() && hit.transform.GetComponent<MovingPlatformHorizontalSpikes>().platformType == MovingPlatformHorizontalSpikes.PlatformType.vertical)
                        {
                            if (directionY == 1 || hit.distance == 0)
                                continue;
                            if (collisions.fallingThroughPlatform)
                                continue;
                            if (directionalInput.y == -1)
                            {
                                collisions.fallingThroughPlatform = true;
                                StartCoroutine(ResetFallingThroughPlatform());
                                continue;
                            }
                        }
                    }
                }
                else if (hit.collider.tag == K.TAG_PASSABLE)
                {
                    if (directionY == 1 || hit.distance == 0)
                        continue;
                    if (collisions.fallingThroughPlatform)
                        continue;
                    if (directionalInput.y == -1)
                    {
                        collisions.fallingThroughPlatform = true;
                        StartCoroutine(ResetFallingThroughPlatform());
                        continue;
                    }
                }

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLenght = hit.distance;

                if (directionY == 1)
                    collisions.up = true;
                if (directionY == -1)
                    collisions.down = true;
            }
        }
    }

    IEnumerator ResetFallingThroughPlatform()
    {
        yield return new WaitForEndOfFrame();
        collisions.fallingThroughPlatform = false;
    }

    public struct CollisionInfo
    {
        public Vector2 previousVelocity;
        public bool up;
        public bool right;
        public bool left;
        public bool down;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            up = false;
            down = false;
            right = false;
            left = false;
        }
    }
}