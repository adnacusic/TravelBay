import 'dart:convert';
import 'dart:typed_data';

import 'package:http/http.dart' as http;

import '../models/user.dart';
import '../models/user_activity.dart';
import '../utils/image_files.dart';
import 'api_provider.dart';

/// The signed-in user's own account (Users/Me); a user never reads other accounts.
class UserProvider extends ApiProvider {
  UserProvider() : super('Users');

  /// Profile picture bytes, kept per Asset id so the image is fetched (and decoded) once.
  int? _cachedImageId;
  Uint8List? _cachedImage;

  Future<User> getMe() async => User.fromJson(await getJson('$endpoint/Me'));

  Future<UserActivity> getActivity() async =>
      UserActivity.fromJson(await getJson('$endpoint/Me/Activity'));

  Future<User> updateMe(Map<String, dynamic> request) async =>
      User.fromJson((await sendAction('PUT', '$endpoint/Me', request))!);

  Future<void> changePassword({
    required String password,
    required String newPassword,
    required String confirmNewPassword,
  }) =>
      sendAction('PUT', '$endpoint/Me/ChangePassword', {
        'password': password,
        'newPassword': newPassword,
        'confirmNewPassword': confirmNewPassword,
      });

  /// The picture is behind auth, so it is loaded through the provider (token refresh included).
  Future<Uint8List?> profileImage(User user) async {
    final imageId = user.profileImageId;
    if (imageId == null) {
      return null;
    }
    if (imageId == _cachedImageId) {
      return _cachedImage;
    }
    final response = await send(
      (headers) => http.get(buildUri('$endpoint/Me/ProfileImage'), headers: headers),
    );
    _cachedImageId = imageId;
    _cachedImage = response.bodyBytes;
    return _cachedImage;
  }

  Future<User> setProfileImage(PickedImage image) async {
    final user = User.fromJson((await sendAction('PUT', '$endpoint/Me/ProfileImage', {
      'fileName': image.fileName,
      'contentType': image.contentType,
      'base64Content': image.base64Content,
    }))!);
    _cachedImageId = user.profileImageId;
    _cachedImage = image.bytes;
    notifyListeners();
    return user;
  }

  Future<User> removeProfileImage() async {
    final response = await send(
      (headers) => http.delete(buildUri('$endpoint/Me/ProfileImage'), headers: headers),
    );
    _cachedImageId = null;
    _cachedImage = null;
    notifyListeners();
    return User.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
  }
}
