// public class PID {

// void gaussBlur_4 (float[,] scl,float[,] tcl,int w,int h,int r) {
//     var bxs = boxesForGauss(r, 3);
//     boxBlur_4 (scl, tcl, w, h, (bxs[0]-1)/2);
//     boxBlur_4 (tcl, scl, w, h, (bxs[1]-1)/2);
//     boxBlur_4 (scl, tcl, w, h, (bxs[2]-1)/2);
// }
// void boxBlur_4 (float[,] scl,float[,] tcl,int w,int h,int r) {
//     for(var i=0; i<scl.Length; i++) tcl[i] = scl[i];
//     boxBlurH_4(tcl, scl, w, h, r);
//     boxBlurT_4(float[,] scl,float[,] tcl,int w,int h,int r);
// }
// void boxBlurH_4 (float[,] scl,float[,] tcl,int w,int h,int r) {
//     var iarr = 1 / (r+r+1);
//     for(var i=0; i<h; i++) {
//         var ti = i*w;
//         var li = ti;
//         var ri = ti+r;
//         var fv = scl[ti];
//         var lv = scl[ti+w-1];
//         var val = (r+1)*fv;
//         for(var j=0; j<r; j++) val += scl[ti+j];
//         for(var j=0  ; j<=r ; j++) { val += scl[ri++] - fv       ;   tcl[ti++] = Math.round(val*iarr); }
//         for(var j=r+1; j<w-r; j++) { val += scl[ri++] - scl[li++];   tcl[ti++] = Math.round(val*iarr); }
//         for(var j=w-r; j<w  ; j++) { val += lv        - scl[li++];   tcl[ti++] = Math.round(val*iarr); }
//     }
// }
// void boxBlurT_4 (float[,] scl,float[,] tcl,int w,int h,int r) {
//     var iarr = 1 / (r+r+1);
//     for(var i=0; i<w; i++) {
//         var ti = i, li = ti, ri = ti+r*w;
//         var fv = scl[ti], lv = scl[ti+w*(h-1)], val = (r+1)*fv;
//         for(var j=0; j<r; j++) val += scl[ti+j*w];
//         for(var j=0  ; j<=r ; j++) { val += scl[ri] - fv     ;  tcl[ti] = Math.round(val*iarr);  ri+=w; ti+=w; }
//         for(var j=r+1; j<h-r; j++) { val += scl[ri] - scl[li];  tcl[ti] = Math.round(val*iarr);  li+=w; ri+=w; ti+=w; }
//         for(var j=h-r; j<h  ; j++) { val += lv      - scl[li];  tcl[ti] = Math.round(val*iarr);  li+=w; ti+=w; }
//     }
// }}