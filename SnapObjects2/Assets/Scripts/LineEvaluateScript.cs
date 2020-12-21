using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineEvaluateScript : MonoBehaviour {
    public Text similarityText;
    public GameObject referenceCube;

    public bool CompareLines (LineRenderer lineRenderer1, LineRenderer lineRenderer2) {
        Vector3[] lineBuffer1 = new Vector3[lineRenderer1.positionCount];
        Vector3[] lineBuffer2 = new Vector3[lineRenderer2.positionCount];

        lineRenderer1.GetPositions (lineBuffer1);
        lineRenderer2.GetPositions (lineBuffer2);

        Vector3[] rotatedPoints1 = FindRotatedPoints (lineBuffer1);
        Vector3[] rotatedPoints2 = FindRotatedPoints (lineBuffer2);

        Vector3[] offsets1 = FindOffsets (rotatedPoints1);
        Vector3[] offsets2 = FindOffsets (rotatedPoints2);

        float diff = DifferenceBetweenLines (offsets1, offsets2);
        const float threshold = 2f;

        string result = diff < threshold ? "Pretty close!" : "Not that close...";
        similarityText.text = "Similarity Score: " + diff + ". " + result;
        Debug.Log (diff);
        Debug.Log (diff < threshold ? "Pretty close!" : "Not that close...");
        return diff < threshold;
    }

    private Vector3[] FindOffsets (Vector3[] linePoints) {
        Vector3[] offsets = new Vector3[linePoints.Length];
        Vector3 firstPoint = linePoints[0];
        for (int i = 0; i < linePoints.Length; i++) {
            Vector3 point = linePoints[i];
            offsets[i] = point - firstPoint;
            Debug.Log (offsets[i]);
        }

        return offsets;
    }

    private Vector3[] FindRotatedPoints (Vector3[] bufferLine) {
        Debug.Log ("start");
        Vector3 direction = bufferLine[0] - referenceCube.transform.position;
        Vector3 original = -referenceCube.transform.forward;

        Quaternion angle = Quaternion.FromToRotation (direction, original);
        Vector3[] rotatedPoints = new Vector3[bufferLine.Length];

        Vector3 firstPoint = bufferLine[0] - referenceCube.transform.position;
        Vector3 equalizer = firstPoint - new Vector3 (0, 0, -1);

        for (int i = 0; i < rotatedPoints.Length; i++) {
            Vector3 origPoint = bufferLine[i] - referenceCube.transform.position;
            Vector3 rotatedPoint = angle * origPoint;

            rotatedPoints[i] = rotatedPoint - equalizer;

        }
        return rotatedPoints;
        // Vector3 newVec = angle * direction;
        // Debug.Log ("direction: " + direction);
        // Debug.Log ("newVec: " + newVec);
        // Vector3 second = bufferLine[2] - referenceCube.transform.position;
        // Vector3 secondResult = angle * second;
        // Debug.Log ("secondRes: " + secondResult);

        // Debug.Log ("Distance 1 : " + Vector3.Distance (bufferLine[0], bufferLine[2]));
        // Debug.Log ("Distance 2 : " + Vector3.Distance (newVec, secondResult));
        // float xAngle = Vector3.SignedAngle (direction, original, Vector3.right);
        // float yAngle = Vector3.SignedAngle (direction, original, Vector3.up);
        // float zAngle = Vector3.SignedAngle (direction, original, Vector3.forward);
        // Debug.Log ("Xangle: " + xAngle);
        // Debug.Log ("Yangle: " + yAngle);
        // Debug.Log ("Zangle: " + zAngle);

    }

    float DifferenceBetweenLines (Vector3[] drawn, Vector3[] toMatch) {
        float sqrDistAcc = 0f;
        float length = 0f;

        Vector3 prevPoint = toMatch[0];

        foreach (var toMatchPoint in WalkAlongLine (toMatch)) {
            sqrDistAcc += SqrDistanceToLine (drawn, toMatchPoint);
            length += Vector3.Distance (toMatchPoint, prevPoint);

            prevPoint = toMatchPoint;
        }

        return sqrDistAcc / length;
    }

    /// <summary>
    /// Move a point from the beginning of the line to its end using a maximum step, yielding the point at each step.
    /// </summary>
    IEnumerable<Vector3> WalkAlongLine (IEnumerable<Vector3> line, float maxStep = .1f) {
        using (var lineEnum = line.GetEnumerator ()) {
            if (!lineEnum.MoveNext ())
                yield break;

            var pos = lineEnum.Current;

            while (lineEnum.MoveNext ()) {
                // Debug.Log (lineEnum.Current);
                var target = lineEnum.Current;
                while (pos != target) {
                    yield return pos = Vector3.MoveTowards (pos, target, maxStep);
                }
            }
        }
    }

    static float SqrDistanceToLine (Vector3[] line, Vector3 point) {
        return ListSegments (line)
            .Select (seg => SqrDistanceToSegment (seg.a, seg.b, point))
            .Min ();
    }

    static float SqrDistanceToSegment (Vector3 linePoint1, Vector3 linePoint2, Vector3 point) {
        var projected = ProjectPointOnLineSegment (linePoint1, linePoint1, point);
        return (projected - point).sqrMagnitude;
    }

    /// <summary>
    /// Outputs each position of the line (but the last) and the consecutive one wrapped in a Segment.
    /// Example: a, b, c, d --> (a, b), (b, c), (c, d)
    /// </summary>
    static IEnumerable<Segment> ListSegments (IEnumerable<Vector3> line) {
        using (var pt1 = line.GetEnumerator ())
        using (var pt2 = line.GetEnumerator ()) {
            pt2.MoveNext ();

            while (pt2.MoveNext ()) {
                pt1.MoveNext ();

                yield return new Segment { a = pt1.Current, b = pt2.Current };
            }
        }
    }
    struct Segment {
        public Vector3 a;
        public Vector3 b;
    }

    //This function finds out on which side of a line segment the point is located.
    //The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
    //the line segment, project it on the line using ProjectPointOnLine() first.
    //Returns 0 if point is on the line segment.
    //Returns 1 if point is outside of the line segment and located on the side of linePoint1.
    //Returns 2 if point is outside of the line segment and located on the side of linePoint2.
    static int PointOnWhichSideOfLineSegment (Vector3 linePoint1, Vector3 linePoint2, Vector3 point) {
        Vector3 lineVec = linePoint2 - linePoint1;
        Vector3 pointVec = point - linePoint1;

        if (Vector3.Dot (pointVec, lineVec) > 0) {
            return pointVec.magnitude <= lineVec.magnitude ? 0 : 2;
        } else {
            return 1;
        }
    }

    //This function returns a point which is a projection from a point to a line.
    //The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
    static Vector3 ProjectPointOnLine (Vector3 linePoint, Vector3 lineVec, Vector3 point) {
        //get vector from point on line to point in space
        Vector3 linePointToPoint = point - linePoint;
        float t = Vector3.Dot (linePointToPoint, lineVec);
        return linePoint + lineVec * t;
    }

    //This function returns a point which is a projection from a point to a line segment.
    //If the projected point lies outside of the line segment, the projected point will
    //be clamped to the appropriate line edge.
    //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
    static Vector3 ProjectPointOnLineSegment (Vector3 linePoint1, Vector3 linePoint2, Vector3 point) {
        Vector3 vector = linePoint2 - linePoint1;
        Vector3 projectedPoint = ProjectPointOnLine (linePoint1, vector.normalized, point);

        switch (PointOnWhichSideOfLineSegment (linePoint1, linePoint2, projectedPoint)) {
            case 0:
                return projectedPoint;
            case 1:
                return linePoint1;
            case 2:
                return linePoint2;
            default:
                //output is invalid
                return Vector3.zero;
        }
    }
}