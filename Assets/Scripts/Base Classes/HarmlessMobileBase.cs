using UnityEngine;
using System.Collections.Generic;

public abstract class HarmlessMobileBase : MobileBase
{
    public LayerMask movableObjectsMask;

    private List<MovableObject> _movableObjects;

    public struct PlatformType
    {
        public static string vertical = "Vertical";
        public static string horizontal = "Horizontal";
    }

    protected override void MoveMovableObject()
    {
        foreach (var passenger in _movableObjects)
        {
            if (passenger.transform != null)
            {
                if (passenger.transform.GetComponent<EntityMotor>())
                {
                    var entityMotor = passenger.transform.GetComponent<EntityMotor>();

                    if (entityMotor.currentGravity == Gravity.DOWN || entityMotor.currentGravity == Gravity.LEFT)
                        entityMotor.Move(passenger.velocity, true);
                    else if (entityMotor.currentGravity == Gravity.UP || entityMotor.currentGravity == Gravity.RIGHT)
                        entityMotor.Move(-passenger.velocity, true);
                }
            }

        }
    }

    protected override void CalculateMovableObjectsMovement()
    {
        HashSet<Transform> touchedTransforms = new HashSet<Transform>();
        _movableObjects = new List<MovableObject>();

        var directionX = Mathf.Sign(velocity.x);
        var directionY = Mathf.Sign(velocity.y);
        var rayLenghtVertical = Mathf.Abs(velocity.y) + skinWidth * 2;
        var rayLenghtHorizontal = Mathf.Abs(velocity.x) + skinWidth * 2;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            var rayOriginUp = raycastOrigins.topLeft;
            var rayOriginDown = raycastOrigins.bottomLeft;
            rayOriginUp += Vector2.right * (horizontalRaySpacing * i);
            rayOriginDown += Vector2.right * (horizontalRaySpacing * i);

            var hitsUp = Physics2D.RaycastAll(rayOriginUp, Vector2.up, rayLenghtVertical, movableObjectsMask);
            if (hitsUp.Length > 0)
            {
                foreach (var hitUp in hitsUp)
                {
                    if (hitUp.distance != 0)
                    {
                        if (!touchedTransforms.Contains(hitUp.transform))
                        {
                            if (hitUp.transform.GetComponent<EntityMotor>())
                            {
                                touchedTransforms.Add(hitUp.transform);

                                var entityMotor = hitUp.transform.GetComponent<EntityMotor>();

                                if (entityMotor.currentGravity == Gravity.DOWN)
                                {
                                    var pushX = velocity.x;
                                    var pushY = velocity.y;

                                    if (hitUp.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitUp.collider.GetComponent<Player>();

                                        if (player.directionalInput.x == directionX)
                                        {
                                            if (directionX == 1)
                                                player.velocity.x -= velocity.x * player.velocity.x;
                                            else if (directionX == -1)
                                                player.velocity.x += velocity.x * player.velocity.x;
                                        }
                                    }

                                    _movableObjects.Add(new MovableObject(hitUp.transform, new Vector2(pushX, pushY)));
                                }
                                else if (entityMotor.currentGravity == Gravity.RIGHT || entityMotor.currentGravity == Gravity.LEFT)
                                {
                                    if (hitUp.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitUp.collider.GetComponent<Player>();
                                        if (player.collisions.down)
                                        {
                                            var pushX = 0f;
                                            var pushY = velocity.x;

                                            if (directionY == 1)
                                                pushX = velocity.y * -1;
                                            if (directionX == 1)
                                                pushY = 0f;

                                            _movableObjects.Add(new MovableObject(hitUp.transform, new Vector2(pushX, pushY)));
                                        }
                                        else
                                        {
                                            player.movingPlatformSliding = true;
                                            player.platformVelocityX = 0;

                                            if (velocity.y > 0)
                                            {
                                                if (player.currentGravity == Gravity.LEFT)
                                                {
                                                    player.collisions.right = true;
                                                    player.wallDirX = 1;
                                                }
                                                else if (player.currentGravity == Gravity.RIGHT)
                                                {
                                                    player.collisions.left = true;
                                                    player.wallDirX = -1;
                                                }
                                                player.platformVelocityX = velocity.y;

                                                var pushX = 0f;
                                                var pushY = 0f;

                                                if (directionY == 1)
                                                    pushX = velocity.y * -1;

                                                _movableObjects.Add(new MovableObject(hitUp.transform, new Vector2(pushX, pushY)));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (directionY == 1)
                                        {
                                            var pushX = velocity.y * -1;
                                            var pushY = velocity.x;

                                            _movableObjects.Add(new MovableObject(hitUp.transform, new Vector2(pushX, pushY)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var hitsDown = Physics2D.RaycastAll(rayOriginDown, Vector2.down, rayLenghtVertical, movableObjectsMask);
            if (hitsDown.Length > 0)
            {
                foreach (var hitDown in hitsDown)
                {
                    if (hitDown.distance != 0)
                    {
                        if (!touchedTransforms.Contains(hitDown.transform))
                        {
                            if (hitDown.transform.GetComponent<EntityMotor>())
                            {
                                touchedTransforms.Add(hitDown.transform);

                                var entityMotor = hitDown.transform.GetComponent<EntityMotor>();

                                if (entityMotor.currentGravity == Gravity.UP)
                                {
                                    var pushX = velocity.x;
                                    var pushY = velocity.y;

                                    if (hitDown.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitDown.collider.GetComponent<Player>();

                                        if (player.directionalInput.x != directionX)
                                        {
                                            if (directionX == 1)
                                                player.velocity.x -= velocity.x * player.velocity.x;
                                            else if (directionX == -1)
                                                player.velocity.x += velocity.x * player.velocity.x;
                                        }
                                    }

                                    _movableObjects.Add(new MovableObject(hitDown.transform, new Vector2(pushX, pushY)));
                                }
                                else if (entityMotor.currentGravity == Gravity.RIGHT || entityMotor.currentGravity == Gravity.LEFT)
                                {
                                    if (hitDown.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitDown.collider.GetComponent<Player>();
                                        if (player.collisions.down)
                                        {
                                            var pushX = 0f;
                                            var pushY = velocity.x;

                                            if (directionY == -1)
                                                pushX = velocity.y * -1;
                                            if (directionX == 1)
                                                pushY = 0f;

                                            _movableObjects.Add(new MovableObject(hitDown.transform, new Vector2(pushX, pushY)));
                                        }
                                        else
                                        {
                                            player.movingPlatformSliding = true;
                                            player.platformVelocityX = 0;

                                            if (velocity.y < 0)
                                            {
                                                if (player.currentGravity == Gravity.LEFT)
                                                {
                                                    player.collisions.left = true;
                                                    player.wallDirX = -1;
                                                }
                                                else if (player.currentGravity == Gravity.RIGHT)
                                                {
                                                    player.collisions.right = true;
                                                    player.wallDirX = 1;
                                                }
                                                player.platformVelocityX = velocity.y * -1;

                                                var pushX = 0f;
                                                var pushY = 0f;

                                                if (directionY == -1)
                                                    pushX = velocity.y * -1;

                                                _movableObjects.Add(new MovableObject(hitDown.transform, new Vector2(pushX, pushY)));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (directionY == -1)
                                        {
                                            var pushX = velocity.y * -1;
                                            var pushY = velocity.x;

                                            _movableObjects.Add(new MovableObject(hitDown.transform, new Vector2(pushX, pushY)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < verticalRayCount; i++)
        {
            var rayOriginRight = raycastOrigins.bottomRight;
            var rayOriginLeft = raycastOrigins.bottomLeft;
            rayOriginRight += Vector2.up * (verticalRaySpacing * i);
            rayOriginLeft += Vector2.up * (verticalRaySpacing * i);

            var hitsRight = Physics2D.RaycastAll(rayOriginRight, Vector2.right, rayLenghtHorizontal, movableObjectsMask);
            if (hitsRight.Length > 0)
            {
                foreach (var hitRight in hitsRight)
                {
                    if (hitRight.distance != 0)
                    {
                        if (!touchedTransforms.Contains(hitRight.transform))
                        {
                            if (hitRight.transform.GetComponent<EntityMotor>())
                            {
                                touchedTransforms.Add(hitRight.transform);

                                var entityMotor = hitRight.transform.GetComponent<EntityMotor>();

                                if (entityMotor.currentGravity == Gravity.UP || entityMotor.currentGravity == Gravity.DOWN)
                                {
                                    if (hitRight.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitRight.collider.GetComponent<Player>();
                                        if (player.collisions.down)
                                        {
                                            var pushX = 0f;
                                            var pushY = velocity.y;

                                            if (directionX == 1)
                                                pushX = velocity.x;
                                            if (directionY == 1)
                                                pushY = 0f;

                                            _movableObjects.Add(new MovableObject(hitRight.transform, new Vector2(pushX, pushY)));
                                        }
                                        else
                                        {
                                            player.movingPlatformSliding = true;
                                            player.platformVelocityX = 0;

                                            if (velocity.x > 0)
                                            {
                                                if (player.currentGravity == Gravity.UP)
                                                {
                                                    player.collisions.right = true;
                                                    player.wallDirX = 1;
                                                }
                                                else if (player.currentGravity == Gravity.DOWN)
                                                {
                                                    player.collisions.left = true;
                                                    player.wallDirX = -1;
                                                }
                                                player.platformVelocityX = velocity.x;

                                                var pushX = 0f;
                                                var pushY = 0f;

                                                if (directionX == 1)
                                                    pushX = velocity.x;

                                                _movableObjects.Add(new MovableObject(hitRight.transform, new Vector2(pushX, pushY)));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (directionX == 1)
                                        {
                                            var pushX = velocity.x;
                                            var pushY = velocity.y;

                                            _movableObjects.Add(new MovableObject(hitRight.transform, new Vector2(pushX, pushY)));
                                        }
                                    }
                                }
                                else if (entityMotor.currentGravity == Gravity.LEFT)
                                {
                                    var pushX = velocity.y * -1;
                                    var pushY = velocity.x;

                                    if (hitRight.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitRight.collider.GetComponent<Player>();

                                        if (player.directionalInput.x != directionY)
                                        {
                                            if (directionY == 1)
                                                player.velocity.x -= velocity.y * player.velocity.x;
                                            else if (directionY == -1)
                                                player.velocity.x += velocity.y * player.velocity.x;
                                        }
                                    }

                                    _movableObjects.Add(new MovableObject(hitRight.transform, new Vector2(pushX, pushY)));
                                }
                            }
                        }
                    }
                }
            }

            var hitsLeft = Physics2D.RaycastAll(rayOriginLeft, Vector2.left, rayLenghtHorizontal, movableObjectsMask);
            if (hitsLeft.Length > 0)
            {
                foreach (var hitLeft in hitsLeft)
                {
                    if (hitLeft.distance != 0)
                    {
                        if (!touchedTransforms.Contains(hitLeft.transform))
                        {
                            if (hitLeft.transform.GetComponent<EntityMotor>())
                            {
                                touchedTransforms.Add(hitLeft.transform);

                                var entityMotor = hitLeft.transform.GetComponent<EntityMotor>();

                                if (entityMotor.currentGravity == Gravity.UP || entityMotor.currentGravity == Gravity.DOWN)
                                {
                                    if (hitLeft.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitLeft.collider.GetComponent<Player>();
                                        if (player.collisions.down)
                                        {
                                            var pushX = 0f;
                                            var pushY = velocity.y;

                                            if (directionX == -1)
                                                pushX = velocity.x;
                                            if (directionY == 1)
                                                pushY = 0f;

                                            _movableObjects.Add(new MovableObject(hitLeft.transform, new Vector2(pushX, pushY)));
                                        }
                                        else
                                        {
                                            player.movingPlatformSliding = true;
                                            player.platformVelocityX = 0;

                                            if (velocity.x < 0)
                                            {
                                                if (player.currentGravity == Gravity.UP)
                                                {
                                                    player.collisions.left = true;
                                                    player.wallDirX = -1;
                                                }
                                                else if (player.currentGravity == Gravity.DOWN)
                                                {
                                                    player.collisions.right = true;
                                                    player.wallDirX = 1;
                                                }
                                                player.platformVelocityX = velocity.x * -1;

                                                var pushX = 0f;
                                                var pushY = 0f;

                                                if (directionX == -1)
                                                    pushX = velocity.x;

                                                _movableObjects.Add(new MovableObject(hitLeft.transform, new Vector2(pushX, pushY)));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (directionX == -1)
                                        {
                                            var pushX = velocity.x;
                                            var pushY = velocity.y;

                                            _movableObjects.Add(new MovableObject(hitLeft.transform, new Vector2(pushX, pushY)));
                                        }
                                    }
                                }
                                else if (entityMotor.currentGravity == Gravity.RIGHT)
                                {
                                    var pushX = velocity.y * -1;
                                    var pushY = velocity.x;

                                    if (hitLeft.collider.gameObject.layer == K.LAYER_PLAYER)
                                    {
                                        var player = hitLeft.collider.GetComponent<Player>();

                                        if (player.directionalInput.x == directionY)
                                        {
                                            if (directionY == 1)
                                                player.velocity.x -= velocity.y * player.velocity.x;
                                            else if (directionY == -1)
                                                player.velocity.x += velocity.y * player.velocity.x;
                                        }
                                    }

                                    _movableObjects.Add(new MovableObject(hitLeft.transform, new Vector2(pushX, pushY)));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    protected struct MovableObject
    {
        public Transform transform;
        public Vector2 velocity;

        public MovableObject(Transform _transform, Vector2 _velocity)
        {
            transform = _transform;
            velocity = _velocity;
        }
    }
}